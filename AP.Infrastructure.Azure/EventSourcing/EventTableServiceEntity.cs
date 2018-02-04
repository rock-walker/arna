namespace AP.Infrastructure.Azure.EventSourcing
{
    using Microsoft.WindowsAzure.Storage.Table;

    public interface IEventRecord
    {
        string PartitionKey { get; }
        string RowKey { get; }
        string SourceId { get; set; }
        string SourceType { get; }
        string Payload { get; }
        string CreationDate { get; }
        string CorrelationId { get; }

        // Standard metadata
        string AssemblyName { get; }
        string Namespace { get; }
        string FullName { get; }
        string TypeName { get; }
    }

    public class EventTableServiceEntity : TableEntity, IEventRecord
    {
        public string SourceId { get; set; }
        public string SourceType { get; set; }
        public string Payload { get; set; }
        public string CreationDate { get; set; }
        public string CorrelationId { get; set; }

        // Standard metadata
        public string AssemblyName { get; set; }
        public string Namespace { get; set; }
        public string FullName { get; set; }
        public string TypeName { get; set; }
    }
}