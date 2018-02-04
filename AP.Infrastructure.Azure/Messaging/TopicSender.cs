namespace AP.Infrastructure.Azure.Messaging
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implements an asynchronous sender of messages to a Windows Azure Service Bus topic.
    /// </summary>
    public class TopicSender : IMessageSender
    {
        private readonly Uri serviceUri;
        private readonly ServiceBusSettings settings;
        private readonly string topic;
        private readonly RetryPolicy retryPolicy;
        private readonly TopicClient topicClient;
        private readonly ILogger<TopicSender> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TopicSender"/> class, 
        /// automatically creating the given topic if it does not exist.
        /// </summary>
        public TopicSender(ServiceBusSettings settings, string topic, ILogger<TopicSender> logger)
        {
            this.settings = settings;
            this.topic = topic;
            this.logger = logger;

            //TODO: verify how does it work. Was changed to newest version of ServiceBus library
            retryPolicy = RetryPolicy.Default;
            topicClient = new TopicClient(settings.ConnectionString, topic, retryPolicy);
        }

        /// <summary>
        /// Notifies that the sender is retrying due to a transient fault.
        /// </summary>
        public event EventHandler Retrying;

        /// <summary>
        /// Asynchronously sends the specified message.
        /// </summary>
        public void SendAsync(Func<Message> messageFactory)
        {
            // TODO: SendAsync is not currently being used by the app or infrastructure.
            // Consider removing or have a callback notifying the result.
            // Always send async.
            this.SendAsync(messageFactory, () => { }, ex => { });
        }

        public void SendAsync(IEnumerable<Func<Message>> messageFactories)
        {
            // TODO: batch/transactional sending?
            foreach (var messageFactory in messageFactories)
            {
                this.SendAsync(messageFactory);
            }
        }

        public async void SendAsync(Func<Message> messageFactory, Action successCallback, Action<Exception> exceptionCallback)
        {
            try
            {
                await retryPolicy.RunOperation(
                    async () =>
                       await topicClient.SendAsync(messageFactory()).ConfigureAwait(false)
                       , TimeSpan.FromMinutes(1)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError("An unrecoverable error occurred while trying to send a message:\r\n{0}", ex);
            }
        }

        public void Send(Func<Message> messageFactory)
        {
            var resetEvent = new ManualResetEvent(false);
            Exception exception = null;

            this.SendAsync(
                messageFactory,
                () => resetEvent.Set(),
                ex =>
                {
                    exception = ex;
                    resetEvent.Set();
                });

            resetEvent.WaitOne();
            if (exception != null)
            {
                throw exception;
            }
        }
    }
}
