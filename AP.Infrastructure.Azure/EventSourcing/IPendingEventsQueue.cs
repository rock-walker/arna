namespace AP.Infrastructure.Azure.EventSourcing
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an event store that contains events that are marked as pending for publishing.
    /// </summary>
    public interface IPendingEventsQueue
    {
        /// <summary>
        /// Gets the pending events for publishing asynchronously using delegate continuations.
        /// </summary>
        /// <param name="partitionKey">The partition key to get events from.</param>
        /// <param name="successCallback">The callback that will be called if the data is successfully retrieved. 
        /// The first argument of the callback is the list of pending events.
        /// The second argument is true if there are more records that were not retrieved.</param>
        /// <param name="exceptionCallback">The callback used if there is an exception that does not allow to continue.</param>
        void GetPendingAsync(string partitionKey, Action<IEnumerable<IEventRecord>, bool> successCallback, Action<Exception> exceptionCallback);

        /// <summary>
        /// Deletes the specified pending event from the queue.
        /// </summary>
        /// <param name="partitionKey">The partition key of the event.</param>
        /// <param name="rowKey">The partition key of the event.</param>
        /// <param name="successCallback">The callback that will be called if the data is successfully retrieved.
        /// The argument specifies if the row was deleted. If false, it means that the row did not exist.
        /// </param>
        /// <param name="exceptionCallback">The callback used if there is an exception that does not allow to continue.</param>
        void DeletePendingAsync(string partitionKey, string rowKey, Action<bool> successCallback, Action<Exception> exceptionCallback);

        /// <summary>
        /// Gets the list of all partitions that have pending unpublished events.
        /// </summary>
        /// <returns>The list of all partitions.</returns>
        IEnumerable<string> GetPartitionsWithPendingEvents();

        /// <summary>
        /// Notifies that the sender is retrying due to a transient fault.
        /// </summary>
        event EventHandler Retrying;
    }
}