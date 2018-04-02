namespace AP.Infrastructure.Azure.EventSourcing
{
    using System.Threading;

    /// <summary>
    /// Represents a process that reliably publishes events that are marked as pending in an event store.
    /// </summary>
    public interface IEventStoreBusPublisher
    {
        /// <summary>
        /// Starts processing pending events.
        /// </summary>
        void Start(CancellationToken cancellationToken);

        /// <summary>
        /// Notifies the publisher that there are new pending events in the specified partitionKey.
        /// </summary>
        /// <param name="partitionKey">The partition key or session ID.</param>
        /// <param name="eventCount">A hint that specifies how many new events are pending.</param>
        void SendAsync(string partitionKey, int eventCount);
    }
}