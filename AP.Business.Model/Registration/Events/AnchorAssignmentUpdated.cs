namespace AP.Business.Model.Registration.Events
{
    using System;
    using Infrastructure.EventSourcing;

    public class AnchorAssignmentUpdated : VersionedEvent
    {
        public AnchorAssignmentUpdated(Guid sourceId)
        {
            this.SourceId = sourceId;
        }

        public int Position { get; set; }
        public PersonalInfo Attendee { get; set; }
    }
}
