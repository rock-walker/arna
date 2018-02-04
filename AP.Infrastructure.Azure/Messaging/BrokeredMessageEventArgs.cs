namespace AP.Infrastructure.Azure.Messaging
{
    using System;
    using Microsoft.Azure.ServiceBus;

    /// <summary>
    /// Provides the brokered message payload of an event.
    /// </summary>
    public class BrokeredMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrokeredMessageEventArgs"/> class.
        /// </summary>
        public BrokeredMessageEventArgs(Message message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets the message associated with the event.
        /// </summary>
        public Message Message { get; private set; }

        /// <summary>
        /// Gets or sets an indication that the message should not be disposed by the originating receiver.
        /// </summary>
        public bool DoNotDisposeMessage { get; set; }
    }
}
