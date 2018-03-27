// ==============================================================================================================
namespace AP.Infrastructure.EventSourcing
{
    using System;

    public abstract class VersionedEvent : IVersionedEvent
    {
        public Guid SourceId { get; set; }

        public int Version { get; set; }
    }
}