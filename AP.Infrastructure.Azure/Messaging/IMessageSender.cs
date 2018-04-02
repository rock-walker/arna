namespace AP.Infrastructure.Azure.Messaging
{
    using Microsoft.Azure.ServiceBus;
    using System;

    /// <summary>
    /// Abstracts the behavior of sending a message.
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends the specified message synchronously.
        /// </summary>
        void Send(Func<Message> messageFactory);

        /// <summary>
        /// Sends the specified message asynchronously.
        /// </summary>
        void SendAsync(Func<Message> messageFactory);

        /// <summary>
        /// Sends the specified message asynchronously.
        /// </summary>
        void SendAsync(Func<Message> messageFactory, Action successCallback, Action<Exception> exceptionCallback);

        /// <summary>
        /// Notifies that the sender is retrying due to a transient fault.
        /// </summary>
        event EventHandler Retrying;
    }
}
