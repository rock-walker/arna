// Based on http://windowsazurecat.com/2011/09/best-practices-leveraging-windows-azure-service-bus-brokered-messaging-api/

namespace AP.Infrastructure.Azure.Messaging
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.ServiceBus;
    using Polly.Retry;
    using Polly;
    using Microsoft.Extensions.Logging;
    using AP.Infrastructure.Azure.Utils;

    /// <summary>
    /// Implements an asynchronous receiver of messages from a Windows Azure 
    /// Service Bus topic subscription using sessions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In V3 we made a lot of changes to optimize the performance and scalability of the receiver.
    /// See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more information on the optimizations and migration to V3.
    /// </para>
    /// <para>
    /// The current implementation uses async calls to communicate with the service bus, although the message processing is done with a blocking synchronous call.
    /// We could still make several performance improvements. For example, we could take advantage of sessions and batch multiple messages to avoid accessing the
    /// repositories multiple times where appropriate. See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more potential 
    /// performance and scalability optimizations.
    /// </para>
    /// </remarks>
    public class SessionSubscriptionReceiver : IMessageReceiver, IDisposable
    {
        private static readonly TimeSpan AcceptSessionLongPollingTimeout = TimeSpan.FromMinutes(1);

        private readonly ServiceBusSettings settings;
        private readonly string topic;
        private readonly string subscription;
        private readonly bool requiresSequentialProcessing;
        private readonly object lockObject = new object();
        private readonly Polly.Retry.RetryPolicy receiveRetryPolicy;
        private readonly DynamicThrottling dynamicThrottling;
        private CancellationTokenSource cancellationSource;
        private SubscriptionClient client;
        private ILogger<SessionSubscriptionReceiver> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionSubscriptionReceiver"/> class, 
        /// automatically creating the topic and subscription if they don't exist.
        /// </summary>
        public SessionSubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool requiresSequentialProcessing, ILogger<SessionSubscriptionReceiver> logger)
        {
            this.settings = settings;
            this.topic = topic;
            this.subscription = subscription;
            this.requiresSequentialProcessing = requiresSequentialProcessing;
            this.logger = logger;

            client = new SubscriptionClient(settings.ConnectionString, topic, subscription, ReceiveMode.PeekLock, Microsoft.Azure.ServiceBus.RetryPolicy.Default);

            if (this.requiresSequentialProcessing)
            {
                this.client.PrefetchCount = 10;
            }
            else
            {
                this.client.PrefetchCount = 15;
            }

            this.dynamicThrottling =
                new DynamicThrottling(
                    maxDegreeOfParallelism: 160,
                    minDegreeOfParallelism: 30,
                    penaltyAmount: 3,
                    workFailedPenaltyAmount: 5,
                    workCompletedParallelismGain: 1,
                    intervalForRestoringDegreeOfParallelism: 10000);

            this.receiveRetryPolicy = Policy.Handle<Exception>()
                    .WaitAndRetryAsync(10, retry => TimeSpan.FromSeconds(Math.Pow(2, retry)), 
                        (ex, ts, attempt, context) =>
                        {
                            this.dynamicThrottling.Penalize();
                            logger.LogWarning($"An error occurred in attempt number {attempt} to receive a message from subscription {subscription}: {ex.Message}");
                        });
        }

        /// <summary>
        /// Handler for incoming messages. The return value indicates whether the message should be disposed.
        /// </summary>
        protected Func<Message, MessageReleaseAction> MessageHandler { get; private set; }

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public void Start(Func<Message, MessageReleaseAction> messageHandler)
        {
            lock (this.lockObject)
            {
                // If it's not null, there is already a listening task.
                if (this.cancellationSource == null)
                {
                    this.MessageHandler = messageHandler;
                    this.cancellationSource = new CancellationTokenSource();
                    var cancellationToken = this.cancellationSource.Token;
                    /*
                    client.RegisterSessionHandler(ReceiveMessagesAndCloseSession,
                        new SessionHandlerOptions(evArgs =>
                        {
                            logger.LogError("An unrecoverable error occurred while trying to accept a session in subscription {1}:\r\n{0}",
                            evArgs.Exception.Message, this.subscription);
                            return Task.CompletedTask;
                        })
                        {
                            AutoComplete = false,
                            MaxAutoRenewDuration = TimeSpan.FromSeconds(10),
                            MaxConcurrentSessions = 5
                        }
                        );
                        */
                    Task.Run(() => this.AcceptSession(cancellationToken), cancellationToken);
                    this.dynamicThrottling.Start(this.cancellationSource.Token);
                }
            }
        }

        /// <summary>
        /// Stops the listener.
        /// </summary>
        public void Stop()
        {
            lock (this.lockObject)
            {
                using (this.cancellationSource)
                {
                    if (this.cancellationSource != null)
                    {
                        this.cancellationSource.Cancel();
                        this.cancellationSource = null;
                        this.MessageHandler = null;
                    }
                }
            }
        }

        /// <summary>
        /// Stops the listener if it was started previously.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            this.Stop();

            client.CloseAsync().Wait();

            if (disposing)
            {
                using (this.dynamicThrottling as IDisposable) { }
            }
        }

        protected virtual MessageReleaseAction InvokeMessageHandler(Message message)
        {
            return this.MessageHandler != null ? this.MessageHandler(message) : MessageReleaseAction.AbandonMessage;
        }

        ~SessionSubscriptionReceiver()
        {
            Dispose(false);
        }

        private void AcceptSession(CancellationToken cancellationToken)
        {
            this.dynamicThrottling.WaitUntilAllowedParallelism(cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                // Initialize a custom action acting as a callback whenever a non-transient exception occurs while accepting a session.
                Func<Exception, Task> recoverAcceptSession = ex =>
                {
                    // Just log an exception. Do not allow an unhandled exception to terminate the message receive loop abnormally.
                    logger.LogWarning($"An unrecoverable error occurred while trying to accept a session in subscription \"{subscription}\":\r\n{ex.Message}");
                    this.dynamicThrottling.Penalize();
                    return Task.CompletedTask;

                    /*
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        // Continue accepting new sessions until told to stop regardless of any exceptions.
                        return TaskEx.Delay(10000).ContinueWith(t => AcceptSession(cancellationToken));
                    }
                    else
                    {
                        return Task.FromResult(0);
                    }*/
                };

                try
                {
                    this.dynamicThrottling.NotifyWorkStarted();
                    client.RegisterSessionHandler(ReceiveMessagesAndCloseSession, new SessionHandlerOptions(evArgs =>
                    {
                        return recoverAcceptSession(evArgs.Exception);
                    })
                    {
                        AutoComplete = false,
                        MaxAutoRenewDuration = TimeSpan.FromMinutes(30),
                        MaxConcurrentSessions = 1
                    });
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"The client \"{client.Path}\" already registered. Message is : {ex.Message}");
                }
                /*
                this.receiveRetryPolicy.ExecuteAsync(() => {
                        return Task.Run(
                        () => {
                            back code here
                        });
                    });
                    */
            }
        }

        private Task ReceiveMessagesAndCloseSession(IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            //await this.receiveRetryPolicy.ExecuteAsync
            //(() =>
            //{
                try
                {
                    // Process the message once it was successfully received
                    // Check if we actually received any messages.
                    if (message != null)
                    {
                        var roundtripStopwatch = Stopwatch.StartNew();

                        //return Task.Run(async () =>
                        //{
                        var releaseAction = MessageReleaseAction.AbandonMessage;
                        Task releaseTask = null;
                        try
                        {

                            // Make sure the process was told to stop receiving while it was waiting for a new message.
                            if (!cancellationToken.IsCancellationRequested)
                            {
                                try
                                {
                                    releaseAction = this.InvokeMessageHandler(message);
                                }
                                catch
                                {
                                    throw;
                                }
                                finally
                                {
                                    if (roundtripStopwatch.Elapsed > TimeSpan.FromSeconds(45))
                                    {
                                        this.dynamicThrottling.Penalize();
                                    }
                                }
                            }
                        }
                        finally
                        {
                            // Ensure that any resources allocated by a BrokeredMessage instance are released.
                            if (this.requiresSequentialProcessing)
                            {
                                releaseTask = this.ReleaseMessage(session, message, releaseAction,
                                    () => { },
                                    async () => { await CloseSession(false,session, cancellationToken); },
                                    roundtripStopwatch);
                            }
                            else
                            {
                                // Receives next without waiting for the message to be released.
                                releaseTask = this.ReleaseMessage(session, message, releaseAction,
                                    () => { },
                                    () => { this.dynamicThrottling.Penalize(); },
                                    roundtripStopwatch);
                            }
                        }
                        return releaseTask;
                    }
                    else
                    {
                        // no more messages in the session, close it and do not continue receiving
                        return CloseSession(true, session, cancellationToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"An unrecoverable error occurred while trying to receive a new message from subscription \"{subscription}\":\r\n{ex.Message}");

                    // Cannot continue to receive messages from this session.
                    return CloseSession(false, session, cancellationToken);
                }
            //});
        }

        private Task CloseSession(bool success, IMessageSession session, CancellationToken cancellationToken)
        {
            return receiveRetryPolicy.ExecuteAsync(
                async () =>
                {
                    try
                    {
                        await session.CloseAsync();

                        if (success)
                        {
                            dynamicThrottling.NotifyWorkCompleted();
                        }
                        else
                        {
                            dynamicThrottling.NotifyWorkCompletedWithError();
                        }

                        return Task.CompletedTask;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"An unrecoverable error occurred while trying to CLOSE a session in subscription \"{subscription}\":\r\n{ex.Message}");
                        dynamicThrottling.NotifyWorkCompletedWithError();
                        return Task.CompletedTask;
                    }
                });
        }
        /*
        /// <summary>
        /// Receives the messages in an asynchronous loop and closes the session once there are no more messages.
        /// </summary>
        private async Task ReceiveMessagesAndCloseSession(IMessageSession session, Message message, CancellationToken cancellationToken)
        {
            CountdownEvent unreleasedMessages = new CountdownEvent(1);

            Func<bool, Task> closeSession = async (bool success) =>
            {
                Func<Task> doClose = () =>
                    {
                        try
                        {
                            unreleasedMessages.Signal();
                            if (!unreleasedMessages.Wait(15000, cancellationToken))
                            {
                                logger.LogWarning("Waited for pending unreleased messages before closing session in subscription {0} but they did not complete in time", this.subscription);
                            }
                        }
                        catch (OperationCanceledException)
                        {
                        }
                        finally
                        {
                            unreleasedMessages.Dispose();
                        }

                        return receiveRetryPolicy.ExecuteAsync(
                            async () =>
                            {
                                try {
                                     await session.CloseAsync();

                                    if (success)
                                    {
                                        dynamicThrottling.NotifyWorkCompleted();
                                    }
                                    else
                                    {
                                        dynamicThrottling.NotifyWorkCompletedWithError();
                                    }

                                    return Task.CompletedTask;
                                }
                                catch (Exception ex)
                                {
                                    logger.LogError("An unrecoverable error occurred while trying to close a session in subscription {1}:\r\n{0}", ex, this.subscription);
                                    dynamicThrottling.NotifyWorkCompletedWithError();
                                    return Task.CompletedTask;
                                }
                            });
                    };

                if (this.requiresSequentialProcessing)
                {
                    await doClose();
                }
                else
                {
                    // Allow some time for releasing the messages before closing. Also, continue in a non I/O completion thread in order to block.
                    await Task.Delay(200).ContinueWith(t => doClose());
                }
            };

            // Declare an action to receive the next message in the queue or closes the session if cancelled.
            Func<Message, Task> receiveNext = null;

            // Declare an action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            Func<Exception, Task> recoverReceive = null;

            // Declare an action responsible for the core operations in the message receive loop.
            Func<Message, Task> receiveMessage = ((msg) =>
            {
                // Use a retry policy to execute the Receive action in an asynchronous and reliable fashion.
                //await this.receiveRetryPolicy.ExecuteAsync
                //(
                    //() =>
                    //{
                        try
                        {
                            // Start receiving a new message asynchronously.
                            // Does not wait for new messages to arrive in a session. If no further messages we will just close the session.
                            //var msg = await session.ReceiveAsync(TimeSpan.Zero);

                            // Process the message once it was successfully received
                            // Check if we actually received any messages.
                            if (msg != null)
                            {
                                var roundtripStopwatch = Stopwatch.StartNew();

                                unreleasedMessages.AddCount();

                                //return Task.Run(async () =>
                                //{
                                    var releaseAction = MessageReleaseAction.AbandonMessage;
                                    Task releaseTask = null;
                                    try
                                    {

                                        // Make sure the process was told to stop receiving while it was waiting for a new message.
                                        if (!cancellationToken.IsCancellationRequested)
                                        {
                                            try
                                            {
                                                releaseAction = this.InvokeMessageHandler(msg);
                                            }
                                            catch
                                            {
                                                throw;
                                            }
                                            finally
                                            {
                                                if (roundtripStopwatch.Elapsed > TimeSpan.FromSeconds(45))
                                                {
                                                    this.dynamicThrottling.Penalize();
                                                }
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        // Ensure that any resources allocated by a BrokeredMessage instance are released.
                                        if (this.requiresSequentialProcessing)
                                        {
                                            //releaseTask = session.CompleteAsync(msg.SystemProperties.LockToken);
                                
                                            releaseTask = this.ReleaseMessage(session, msg, releaseAction, 
                                                async () => { await receiveNext(msg); }, 
                                                async () => { await closeSession(false); },
                                                unreleasedMessages, roundtripStopwatch);
                                        }
                                        else
                                        {
                                            // Receives next without waiting for the message to be released.
                                            releaseTask = this.ReleaseMessage(session, msg, releaseAction, 
                                                () => { }, 
                                                () => { this.dynamicThrottling.Penalize(); }, 
                                                unreleasedMessages, roundtripStopwatch);
                                        }
                                    }

                                    return releaseTask;
                                //});
                            }
                            else
                            {
                                // no more messages in the session, close it and do not continue receiving
                                return closeSession(true);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Invoke a custom action to indicate that we have encountered an exception and
                            // need further decision as to whether to continue receiving messages.
                            return recoverReceive(ex);
                        }
                    //});
            });

            // Initialize an action to receive the next message in the queue or closes the session if cancelled.
            receiveNext = async (msg) =>
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    // Continue receiving and processing new messages until told to stop.
                    await receiveMessage(msg);
                }
                else
                {
                    await closeSession(true);
                }
            };

            // Initialize a custom action acting as a callback whenever a non-transient exception occurs while receiving or processing messages.
            recoverReceive = async (ex) =>
            {
                // Just log an exception. Do not allow an unhandled exception to terminate the message receive loop abnormally.
                logger.LogError("An unrecoverable error occurred while trying to receive a new message from subscription {1}:\r\n{0}", ex, this.subscription);

                // Cannot continue to receive messages from this session.
                await closeSession(false);
            };

            // Start receiving messages asynchronously for the session.
            await receiveNext(message);
        }
        */
        private async Task ReleaseMessage(IMessageSession session, Message msg, MessageReleaseAction releaseAction, 
            Action completeReceive, Action onReleaseError, Stopwatch roundtripStopwatch)
        {
            switch (releaseAction.Kind)
            {
                case MessageReleaseActionKind.Complete:
                    await msg.SafeCompleteAsync(
                        this.subscription,
                        session,
                        operationSucceeded =>
                        {
                            if (operationSucceeded)
                            {
                                completeReceive();
                            }
                            else
                            {
                                onReleaseError();
                            }
                        },
                        logger,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.Abandon:
                    this.dynamicThrottling.Penalize();
                    await msg.SafeAbandonAsync(
                        this.subscription,
                        session,
                        succeeded => onReleaseError(),
                        logger,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.DeadLetter:
                    this.dynamicThrottling.Penalize();
                    await msg.SafeDeadLetterAsync(
                        this.subscription,
                        session,
                        releaseAction.DeadLetterReason,
                        releaseAction.DeadLetterDescription,
                        operationSucceeded =>
                        { 
                            if (operationSucceeded)
                            {
                                completeReceive();
                            }
                            else
                            {
                                onReleaseError();
                            }
                        },
                        logger,
                        roundtripStopwatch);
                    break;
                default:
                    break;
            }
        }

        /*
        private void OnMessageCompleted(bool success, CountdownEvent countdown)
        {
            try
            {
                countdown.Signal();
            }
            catch (ObjectDisposedException)
            {
                // It could happen in a rare case that due to a timing issue between closing the session and disposing the countdown,
                // that the countdown is already disposed. This is OK and it can continue processing normally.
            }
        }*/
    }
}
