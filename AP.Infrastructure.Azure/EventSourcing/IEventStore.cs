namespace AP.Infrastructure.Azure.EventSourcing
{
    using System.Collections.Generic;

    public interface IEventStore
    {
        IEnumerable<EventData> Load(string partitionKey, int version);

        void Save(string partitionKey, IEnumerable<EventData> events);
    }
}