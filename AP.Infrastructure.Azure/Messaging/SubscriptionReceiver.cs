namespace AP.Infrastructure.Azure.Messaging
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using AP.Infrastructure.Azure.Utils;
    using Microsoft.Azure.ServiceBus;
    using Polly.Retry;
    using Polly;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implements an asynchronous receiver of messages from a Windows Azure 
    /// Service Bus topic subscription.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In V3 we made a lot of changes to optimize the performance and scalability of the receiver.
    /// See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more information on the optimizations and migration to V3.
    /// </para>
    /// <para>
    /// The current implementation uses async calls to communicate with the service bus, although the message processing is done with a blocking synchronous call.
    /// We could still make several performance improvements. For example, we could react to system-wide throttling indicators to avoid overwhelming
    /// the services when under heavy load. See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more potential 
    /// performance and scalability optimizations.
    /// </para>
    /// </remarks>
    public class SubscriptionReceiver : IMessageReceiver, IDisposable
    {
        private static readonly TimeSpan ReceiveLongPollingTimeout = TimeSpan.FromMinutes(1);

        private readonly ServiceBusSettings settings;
        private readonly string topic;
        //private readonly ISubscriptionReceiverInstrumentation instrumentation;
        private string subscription;
        private readonly object lockObject = new object();
        private readonly Polly.Retry.RetryPolicy receiveRetryPolicy;
        private readonly bool processInParallel;
        private readonly DynamicThrottling dynamicThrottling;
        private CancellationTokenSource cancellationSource;
        private SubscriptionClient client;
        private ILogger<SubscriptionReceiver> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionReceiver"/> class, 
        /// automatically creating the topic and subscription if they don't exist.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Instrumentation disabled in this overload")]
        public SubscriptionReceiver(ServiceBusSettings settings, string topic, string subscription, bool processInParallel, ILogger<SubscriptionReceiver> logger)
        {
            this.settings = settings;
            this.topic = topic;
            this.subscription = subscription;
            this.processInParallel = processInParallel;
            this.logger = logger;

            this.client = new SubscriptionClient(settings.ConnectionString, topic, subscription);
            if (this.processInParallel)
            {
                this.client.PrefetchCount = 18;
            }
            else
            {
                this.client.PrefetchCount = 14;
            }

            dynamicThrottling =
                new DynamicThrottling(
                    maxDegreeOfParallelism: 100,
                    minDegreeOfParallelism: 50,
                    penaltyAmount: 3,
                    workFailedPenaltyAmount: 5,
                    workCompletedParallelismGain: 1,
                    intervalForRestoringDegreeOfParallelism: 8000);

            receiveRetryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(3, (r) => TimeSpan.FromMilliseconds(900),//, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1));
            (ex, ts, attemps, cc) =>
            {
                this.dynamicThrottling.Penalize();
                logger.LogWarning($"An error occurred in attempt number {attemps} to receive a message from subscription {subscription}: {ex.Message}");
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
                this.MessageHandler = messageHandler;
                this.cancellationSource = new CancellationTokenSource();
                var msgHandlerOptions = new MessageHandlerOptions(HandleMessageException)
                {
                    MaxConcurrentCalls = 4,
                    AutoComplete = false,
                };

                this.client.RegisterMessageHandler(ReceiveMessages, msgHandlerOptions);
                this.dynamicThrottling.Start(this.cancellationSource.Token);
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

            if (disposing)
            {
                //using (this.instrumentation as IDisposable) { }
                using (this.dynamicThrottling as IDisposable) { }
            }
        }

        protected virtual MessageReleaseAction InvokeMessageHandler(Message message)
        {
            return this.MessageHandler != null ? this.MessageHandler(message) : MessageReleaseAction.AbandonMessage;
        }

        ~SubscriptionReceiver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Receives the messages in an endless asynchronous loop.
        /// </summary>
        private async Task ReceiveMessages(Message msg, CancellationToken cancellationToken)
        {
            await this.receiveRetryPolicy.ExecuteAsync
            (
                () =>
                {
                    try
                    {
                        Task releaseTask;

                        if (msg != null)
                        {
                            var roundtripStopwatch = Stopwatch.StartNew();

                            var releaseAction = MessageReleaseAction.AbandonMessage;

                            try
                            {
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
                                releaseTask = ReleaseMessage(msg, releaseAction, roundtripStopwatch);
                            }
                        }
                        else
                        {
                            this.dynamicThrottling.NotifyWorkCompleted();
                            return Task.CompletedTask;
                        }

                        return releaseTask;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("An unrecoverable error occurred while trying to receive a new message from subscription {1}:\r\n{0}", ex, this.subscription);
                        this.dynamicThrottling.NotifyWorkCompletedWithError();

                        return Task.CompletedTask;
                    };
                });
        }

        private Task HandleMessageException(ExceptionReceivedEventArgs args)
        {
            logger.LogError($"Message handler in subscription \"{subscription}\" encountered an exception: {args.Exception}");
            return Task.CompletedTask;
        }

        private async Task ReleaseMessage(Message msg, MessageReleaseAction releaseAction, Stopwatch roundtripStopwatch)
        {
            switch (releaseAction.Kind)
            {
                case MessageReleaseActionKind.Complete:
                    await msg.SafeCompleteAsync(
                        this.subscription,
                        client,
                        success =>
                        {
                            if (success)
                            {
                                this.dynamicThrottling.NotifyWorkCompleted();
                            }
                            else
                            {
                                this.dynamicThrottling.NotifyWorkCompletedWithError();
                            }
                        },
                        logger,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.Abandon:
                    await msg.SafeAbandonAsync(
                        this.subscription,
                        client,
                        success => {
                            dynamicThrottling.NotifyWorkCompletedWithError();
                        },
                        logger,
                        roundtripStopwatch);
                    break;
                case MessageReleaseActionKind.DeadLetter:
                    await msg.SafeDeadLetterAsync(
                        this.subscription,
                        client,
                        releaseAction.DeadLetterReason,
                        releaseAction.DeadLetterDescription,
                        success => {
                            this.dynamicThrottling.NotifyWorkCompletedWithError();
                        },
                        logger,
                        roundtripStopwatch);
                    break;
                default:
                    break;
            }
        }
    }
}
