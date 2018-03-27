namespace AP.Business.Model.Registration.Events
{
    using System;
    using Infrastructure.EventSourcing;

    public class AnchorAssigned : VersionedEvent
    {
        public AnchorAssigned(Guid sourceId)
        {
            this.SourceId = sourceId;
        }

        public int Position { get; set; }
        public Guid SeatType { get; set; }
        public PersonalInfo Attendee { get; set; }
    }
}