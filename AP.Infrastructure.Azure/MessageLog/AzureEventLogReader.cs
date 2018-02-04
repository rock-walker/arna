namespace AP.Infrastructure.Azure.MessageLog
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AP.Infrastructure.MessageLog;
    using AP.Infrastructure.Messaging;
    using AP.Infrastructure.Serialization;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;

    public class AzureEventLogReader : IEventLogReader
    {
        private readonly CloudStorageAccount account;
        private readonly string tableName;
        private readonly CloudTableClient tableClient;
        private ITextSerializer serializer;

        public AzureEventLogReader(CloudStorageAccount account, string tableName, ITextSerializer serializer)
        {
            if (account == null) throw new ArgumentNullException("account");
            if (tableName == null) throw new ArgumentNullException("tableName");
            if (string.IsNullOrWhiteSpace(tableName)) throw new ArgumentException("tableName");
            if (serializer == null) throw new ArgumentNullException("serializer");

            this.account = account;
            this.tableName = tableName;
            this.tableClient = account.CreateCloudTableClient();
            this.serializer = serializer;
        }

        // NOTE: we don't have a need (yet?) to query commands, as we don't use them to
        // recreate read models, nor we are using it for BI, so we just 
        // expose events.
        public IEnumerable<IEvent> Query(QueryCriteria criteria)
        {
            var context = tableClient.GetTableReference(tableName);
            TableQuery<MessageLogEntity> query = (new TableQuery<MessageLogEntity>()).Where(TableQuery.GenerateFilterCondition("Kind", QueryComparisons.Equal, StandardMetadata.EventKind));
            
            //TODO: observe this line: convert Expression extension to 'string' FilterCondition
            var where = criteria.ToExpression();
            if (where != null)
            {
                throw new NotImplementedException();
                //query = query.Where(where);
            }

            return context
                .ExecuteQuerySegmentedAsync(query, null)
                .Result
                .Select(e => serializer.Deserialize<IEvent>(e.Payload));
        }
    }
}
