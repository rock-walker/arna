namespace AP.Infrastructure.Azure.EventSourcing
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Polly.Retry;
    using Polly;
    using AutoMapper;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Implements an event store using Windows Azure Table Storage.
    /// </summary>
    /// <remarks>
    /// <para> This class works closely related to <see cref="EventStoreBusPublisher"/> and <see cref="AzureEventSourcedRepository{T}"/>, and provides a resilient mechanism to 
    /// store events, and also manage which events are pending for publishing to an event bus.</para>
    /// <para>Ideally, it would be very valuable to provide asynchronous APIs to avoid blocking I/O calls.</para>
    /// <para>See <see cref="http://go.microsoft.com/fwlink/p/?LinkID=258557"> Journey chapter 7</see> for more potential performance and scalability optimizations.</para>
    /// </remarks>
    public class EventStore : IEventStore, IPendingEventsQueue
    {
        private const string UnpublishedRowKeyPrefix = "Unpublished_";
        private const string UnpublishedRowKeyPrefixUpperLimit = "Unpublished`";
        private const string RowKeyVersionUpperLimit = "9999999999";
        private readonly CloudStorageAccount account;
        private readonly string tableName;
        private readonly CloudTableClient tableClient;
        private readonly RetryPolicy pendingEventsQueueRetryPolicy;
        private readonly RetryPolicy eventStoreRetryPolicy;
        private readonly ILogger<EventStore> logger;

        public EventStore(CloudStorageAccount account, string tableName, ILogger<EventStore> logger)
        {
            if (account == null) throw new ArgumentNullException("account");
            if (tableName == null) throw new ArgumentNullException("tableName");
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("tableName");

            this.account = account;
            this.tableName = tableName;
            this.logger = logger;
            tableClient = account.CreateCloudTableClient();
            tableClient.DefaultRequestOptions = new TableRequestOptions
            {
                RetryPolicy = new Microsoft.WindowsAzure.Storage.RetryPolicies.NoRetry()
            };

            // TODO: This could be injected.
            //var backgroundRetryStrategy = new ExponentialBackoff(10, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(1));
            //var blockingRetryStrategy = new Incremental(3, TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            var blockingRetryStrategy = new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1) };
            pendingEventsQueueRetryPolicy = Policy.Handle<Exception>().WaitAndRetry(blockingRetryStrategy, (ex, ts, countRetry, context) => {
                var handler = this.Retrying;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }

                logger.LogWarning("An error occurred in attempt number {1} to access table storage (PendingEventsQueue): {0}", ex.Message, countRetry);
            });

            eventStoreRetryPolicy = Policy.Handle<Exception>().WaitAndRetry(blockingRetryStrategy, (ex, ts, countRetry, context) => {
                logger.LogWarning("An error occurred in attempt number {1} to access table storage (EventStore): {0}", ex.Message, countRetry);
            });

            this.eventStoreRetryPolicy.Execute(() => {
                var cloudTable = tableClient.GetTableReference(tableName);// CreateTableIfNotExist(tableName)
                cloudTable.CreateIfNotExistsAsync().Wait();
            });
        }

        /// <summary>
        /// Notifies that the sender is retrying due to a transient fault.
        /// </summary>
        public event EventHandler Retrying;

        public IEnumerable<EventData> Load(string partitionKey, int version)
        {
            var minRowKey = version.ToString("D10");
            var query = GetEntitiesQuery(partitionKey, minRowKey, RowKeyVersionUpperLimit);

            // TODO: use async APIs, continuation tokens
            var all = eventStoreRetryPolicy.Execute(() => query.Result);
            return all.Select(x => Mapper.Map(x, new EventData { Version = int.Parse(x.RowKey) }));
        }

        public void Save(string partitionKey, IEnumerable<EventData> events)
        {
            var tableRef = tableClient.GetTableReference(this.tableName);
            var batchOperation = new TableBatchOperation();

            foreach (var eventData in events)
            {
                string creationDate = DateTime.UtcNow.ToString("o");
                var formattedVersion = eventData.Version.ToString("D10");
                var dbEntity = 
                    Mapper.Map(eventData, new EventTableServiceEntity
                        {
                            PartitionKey = partitionKey,
                            RowKey = formattedVersion,
                            CreationDate = creationDate,
                        });
                batchOperation.Insert(dbEntity);
                // Add a duplicate of this event to the Unpublished "queue"
                var dbDuplicateEntity = 
                    Mapper.Map(eventData, new EventTableServiceEntity
                        {
                            PartitionKey = partitionKey,
                            RowKey = UnpublishedRowKeyPrefix + formattedVersion,
                            CreationDate = creationDate,
                        });
                batchOperation.Insert(dbDuplicateEntity);
            }

            try
            {
                eventStoreRetryPolicy.Execute(() => tableRef.ExecuteBatchAsync(batchOperation).Result);
            }
            catch (StorageException ex)
            {
                //var inner = ex.InnerException as //DataServiceClientException;
                var inner = ex.HResult;
                if (inner == (int)HttpStatusCode.Conflict)
                {
                    throw new ConcurrencyException();
                }

                throw;
            }
        }

        /// <summary>
        /// Gets the pending events for publishing asynchronously using delegate continuations.
        /// </summary>
        /// <param name="partitionKey">The partition key to get events from.</param>
        /// <param name="successCallback">The callback that will be called if the data is successfully retrieved. 
        /// The first argument of the callback is the list of pending events.
        /// The second argument is true if there are more records that were not retrieved.</param>
        /// <param name="exceptionCallback">The callback used if there is an exception that does not allow to continue.</param>
        public void GetPendingAsync(string partitionKey, Action<IEnumerable<IEventRecord>, bool> successCallback, Action<Exception> exceptionCallback)
        {
            var query = GetEntitiesQuery(partitionKey, UnpublishedRowKeyPrefix, UnpublishedRowKeyPrefixUpperLimit);
                pendingEventsQueueRetryPolicy
                .Execute(
                    //ac => query.BeginExecuteSegmented(ac, null),
                    //ar => query.EndExecuteSegmented(ar),
                    () =>
                    {
                        var all = query.Result.ToList();
                        successCallback(query.Result, false);
                    }/*,
                    exceptionCallback*/);
        }

        /// <summary>
        /// Deletes the specified pending event from the queue.
        /// </summary>
        /// <param name="partitionKey">The partition key of the event.</param>
        /// <param name="rowKey">The partition key of the event.</param>
        /// <param name="successCallback">The callback that will be called if the data is successfully retrieved.
        /// The argument specifies if the row was deleted. If false, it means that the row did not exist.
        /// </param>
        /// <param name="exceptionCallback">The callback used if there is an exception that does not allow to continue.</param>
        public void DeletePendingAsync(string partitionKey, string rowKey, Action<bool> successCallback, Action<Exception> exceptionCallback)
        {
            var tableRef = this.tableClient.GetTableReference(tableName);
            var item = new EventTableServiceEntity { PartitionKey = partitionKey, RowKey = rowKey };
            TableBatchOperation batch = new TableBatchOperation
            {
                TableOperation.Merge(item),
                TableOperation.Delete(item)
            };

            pendingEventsQueueRetryPolicy.Execute(() =>
                {
                    try
                    {
                        tableRef.ExecuteBatchAsync(batch).Wait();
                    }
                    catch (StorageException ex)
                    {
                        // ignore if entity was already deleted.
                        //var inner = ex.InnerException as DataServiceClientException;
                        if (ex.HResult != (int)HttpStatusCode.NotFound)
                        {
                            throw;
                        }
                    }
                });//,
                //successCallback,
                //exceptionCallback);
        }

        /// <summary>
        /// Gets the list of all partitions that have pending unpublished events.
        /// </summary>
        /// <returns>The list of all partitions.</returns>
        public IEnumerable<string> GetPartitionsWithPendingEvents()
        {
            var tableRef = this.tableClient.GetTableReference(tableName);
            var minKeyFilter = TableQuery.GenerateFilterCondition("rowkey", QueryComparisons.GreaterThanOrEqual, UnpublishedRowKeyPrefix);
            var maxKeyFilter = TableQuery.GenerateFilterCondition("rowkey", QueryComparisons.LessThanOrEqual, UnpublishedRowKeyPrefixUpperLimit);

            var query = (new TableQuery<EventTableServiceEntity>())
                .Where(minKeyFilter)
                .Where(maxKeyFilter)
                .Select(new[] { "PartitionKey" });

            var task = tableRef.ExecuteQuerySegmentedAsync(query, null);

            var result = new BlockingCollection<string>();
            var tokenSource = new CancellationTokenSource();

            this.pendingEventsQueueRetryPolicy.Execute(() =>
                {
                    var events = task.Result;

                    foreach (var key in events.Results.Select(x => x.PartitionKey).Distinct())
                    {
                        result.Add(key);
                    }

                    //TODO: comment temporarily - no time to investigate
                    /*
                    while (events)
                    {
                        try
                        {
                            rs = this.pendingEventsQueueRetryPolicy.ExecuteAction(() => rs.GetNext());
                            foreach (var key in rs.Results.Select(x => x.PartitionKey).Distinct())
                            {
                                result.Add(key);
                            }
                        }
                        catch
                        {
                            // Cancel is to force an exception being thrown in the consuming enumeration thread
                            // TODO: is there a better way to get the correct exception message instead of an OperationCancelledException in the consuming thread?
                            tokenSource.Cancel();
                            throw;
                        }
                    }
                    */
                    result.CompleteAdding();
                }/*,
                ex =>
                {
                    tokenSource.Cancel();
                    throw ex;
                }*/);

            return result.GetConsumingEnumerable(tokenSource.Token);
        }

        private Task<TableQuerySegment<EventTableServiceEntity>> GetEntitiesQuery(string partitionKey, string minRowKey, string maxRowKey)
        {
            var tableRef = tableClient.GetTableReference(tableName);

            var query = new TableQuery<EventTableServiceEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            /*
            var retrieveOperation = TableOperation.Retrieve<EventTableServiceEntity>(partitionKey, minRowKey);
            var partitionKeyCondition = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var rowMinKeyCondition = TableQuery.GenerateFilterCondition("rowkey", QueryComparisons.GreaterThanOrEqual, minRowKey);
            var rowMaxKeyCondition = TableQuery.GenerateFilterCondition("rowkey", QueryComparisons.LessThanOrEqual, maxRowKey);

            var query = (new TableQuery<EventTableServiceEntity>()).Where(partitionKeyCondition).Where(rowMinKeyCondition).Where(rowMaxKeyCondition);
            */
            return tableRef.ExecuteQuerySegmentedAsync(query, null);

            //return task.Result.Result as EventTableServiceEntity;
            /*
            var query = tableRef
                .CreateQuery<EventTableServiceEntity>(tableName)
                .Where(
                    x =>
                    x.PartitionKey == partitionKey && x.RowKey.CompareTo(minRowKey) >= 0 && x.RowKey.CompareTo(maxRowKey) <= 0);

            return query.AsTableServiceQuery();
            */
        }
    }
}
