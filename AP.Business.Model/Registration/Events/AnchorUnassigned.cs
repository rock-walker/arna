namespace AP.Business.Model.Registration.Events
{
    using System;
    using Infrastructure.EventSourcing;

    public class AnchorUnassigned : VersionedEvent
    {
        public AnchorUnassigned(Guid sourceId)
        {
            this.SourceId = sourceId;
        }

        public int Position { get; set; }
    }
}
