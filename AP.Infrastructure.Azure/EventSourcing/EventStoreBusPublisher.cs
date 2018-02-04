﻿namespace AP.Infrastructure.Azure.EventSourcing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using AP.Infrastructure.Azure.Messaging;
    using Microsoft.Azure.ServiceBus;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Publishes events in the <see cref="EventStore"/> to the service bus.
    /// </summary>
    /// <remarks>
    /// This class works closely related to <see cref="EventStore"/> and <see cref="AzureEventSourcedRepository{T}"/>, and provides a resilient mechanism to 
    /// asynchronously publish events to the service bus.
    /// This class parallelizes the sending of events acknowledging possible service throttling, but makes sure that events for the same partition are 
    /// published in order using the same SessionId, so consumers of the events that use subscriptions with sessions are guaranteed to receive the events 
    /// in order within each session.
    /// <para>
    /// We could still make several performance improvements. For example, instead of sending 1 event per <see cref="BrokeredMessage"/> we could
    /// bundle several events for the same session into a single message, reducing the number of I/O calls to both the service bus and table storage, and
    /// it would also avoid waiting for a message to be completed before sending the next message for the same partition. This change would require some
    /// changes in other components and the message metadata as well. See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see>
    /// for more potential performance and scalability optimizations.
    /// </para>
    /// </remarks>
    public class EventStoreBusPublisher : IEventStoreBusPublisher, IDisposable
    {
        private static readonly int RowKeyPrefixIndex = "Unpublished_".Length;
        private readonly IMessageSender sender;
        private readonly IPendingEventsQueue queue;
        private readonly BlockingCollection<string> enqueuedKeys;
        private readonly ILogger<EventStoreBusPublisher> logger;
        private readonly DynamicThrottling dynamicThrottling;

        public EventStoreBusPublisher(IMessageSender sender, 
            IPendingEventsQueue queue, //IEventStoreBusPublisherInstrumentation instrumentation,
            ILogger<EventStoreBusPublisher> logger)
        {
            this.sender = sender;
            this.queue = queue;
            //this.instrumentation = instrumentation;
            this.logger = logger;

            this.enqueuedKeys = new BlockingCollection<string>();
            this.dynamicThrottling = 
                new DynamicThrottling(
                    maxDegreeOfParallelism: 230,
                    minDegreeOfParallelism: 30,
                    penaltyAmount: 3,
                    workFailedPenaltyAmount: 10,
                    workCompletedParallelismGain: 1,
                    intervalForRestoringDegreeOfParallelism: 8000);
            this.queue.Retrying += (s, e) => this.dynamicThrottling.Penalize();
            this.sender.Retrying += (s, e) => this.dynamicThrottling.Penalize();
        }

        public void Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        foreach (var key in GetThrottlingEnumerable(this.enqueuedKeys.GetConsumingEnumerable(cancellationToken), cancellationToken))
                        {
                            if (!cancellationToken.IsCancellationRequested)
                            {
                                ProcessPartition(key);
                            }
                            else
                            {
                                this.EnqueueIfNotExists(key);
                                return;
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                },
                TaskCreationOptions.LongRunning);

            // Query through all partitions to check for pending events, as there could be
            // stored events that were never published before the system was rebooted.
            Task.Factory.StartNew(
                () =>
                {
                    foreach (var partitionKey in this.queue.GetPartitionsWithPendingEvents())
                    {
                        if (cancellationToken.IsCancellationRequested)
                            return;

                        this.EnqueueIfNotExists(partitionKey);
                    }
                },
                TaskCreationOptions.LongRunning);

            this.dynamicThrottling.Start(cancellationToken);
        }

        public void SendAsync(string partitionKey, int eventCount)
        {
            if (string.IsNullOrEmpty(partitionKey))
                throw new ArgumentNullException(partitionKey);

            EnqueueIfNotExists(partitionKey);

            //this.instrumentation.EventsPublishingRequested(eventCount);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.dynamicThrottling.Dispose();
                this.enqueuedKeys.Dispose();
            }
        }

        private void EnqueueIfNotExists(string partitionKey)
        {
            if (!this.enqueuedKeys.Any(partitionKey.Equals))
            {
                // if the key is not already in the queue, add it. No need to add it if it's already there, as
                // when the partition is processed, it will already try to send all events.
                this.enqueuedKeys.Add(partitionKey);
            }
        }

        private void ProcessPartition(string key)
        {
            //this.instrumentation.EventPublisherStarted();

            this.queue.GetPendingAsync(
                key,
                (results, hasMoreResults) =>
                {
                    var enumerator = results.GetEnumerator();
                    this.SendAndDeletePending(
                        enumerator,
                        allElementWereProcessed =>
                        {
                            enumerator.Dispose();
                            if (!allElementWereProcessed)
                            {
                                this.EnqueueIfNotExists(key);
                            }
                            else if (hasMoreResults)
                            {
                                // if there are more events in this partition, then continue processing and do not mark work as completed.
                                ProcessPartition(key);
                                return;
                            }

                            // all elements were processed or should be retried later. Mark this job as done.
                            this.dynamicThrottling.NotifyWorkCompleted();
                            //this.instrumentation.EventPublisherFinished();
                        },
                        ex =>
                        {
                            enumerator.Dispose();
                            logger.LogError("An error occurred while publishing events for partition {0}:\r\n{1}", key, ex);

                            // if there was ANY unhandled error, re-add the item to collection.
                            EnqueueIfNotExists(key);
                            dynamicThrottling.NotifyWorkCompletedWithError();
                            //instrumentation.EventPublisherFinished();
                        });
                },
                ex =>
                {
                    logger.LogError("An error occurred while getting the events pending for publishing for partition {0}:\r\n{1}", key, ex);

                    // if there was ANY unhandled error, re-add the item to collection.
                    EnqueueIfNotExists(key);
                    dynamicThrottling.NotifyWorkCompletedWithError();
                    //instrumentation.EventPublisherFinished();
                });
        }

        private void SendAndDeletePending(IEnumerator<IEventRecord> enumerator, Action<bool> successCallback, Action<Exception> errorCallback)
        {

            Action sendNextEvent = null;
            Action deletePending = null;

            sendNextEvent =
                () =>
                {
                    try
                    {
                        if (enumerator.MoveNext())
                        {
                            var item = enumerator.Current;

                            this.sender.SendAsync(
                                () => BuildMessage(item),
                                deletePending,
                                errorCallback);
                        }
                        else
                        {
                            // no more elements
                            successCallback(true);
                        }
                    }
                    catch (Exception e)
                    {
                        errorCallback(e);
                    }
                };

            deletePending =
                () =>
                {
                    var item = enumerator.Current;
                    this.queue.DeletePendingAsync(
                        item.PartitionKey,
                        item.RowKey,
                        (bool rowDeleted) =>
                        {
                            if (rowDeleted)
                            {
                                //this.instrumentation.EventPublished();

                                sendNextEvent.Invoke();
                            }
                            else
                            {
                                // another thread or process has already sent this event.
                                // stop competing for the same partition and try to send it at the end of the queue if there are any
                                // events still pending.
                                successCallback(false);
                            }
                        },
                        errorCallback);
                };

            sendNextEvent();
        }

        private static Message BuildMessage(IEventRecord record)
        {
            string version = record.RowKey.Substring(RowKeyPrefixIndex);
            var bytes = Encoding.UTF8.GetBytes(record.Payload);
            try
            {
                return new Message(bytes)
                {
                    MessageId = record.PartitionKey + "_" + version,
                    SessionId = record.SourceId,
                    CorrelationId = record.CorrelationId,
                    UserProperties = 
                    {
                        { "Version", version },
                        { StandardMetadata.SourceType, record.SourceType },
                        { StandardMetadata.Kind, StandardMetadata.EventKind },
                        { StandardMetadata.AssemblyName, record.AssemblyName },
                        { StandardMetadata.FullName, record.FullName },
                        { StandardMetadata.Namespace, record.Namespace },
                        { StandardMetadata.SourceId, record.SourceId },
                        { StandardMetadata.TypeName, record.TypeName },
                    }
                };
            }
            catch
            {
                throw;
            }
        }

        private IEnumerable<T> GetThrottlingEnumerable<T>(IEnumerable<T> enumerable, CancellationToken cancellationToken)
        {
            foreach (var item in enumerable)
            {
                this.dynamicThrottling.NotifyWorkStarted();
                yield return item;

                this.dynamicThrottling.WaitUntilAllowedParallelism(cancellationToken);

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }
    }
}
