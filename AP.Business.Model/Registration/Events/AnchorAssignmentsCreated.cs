namespace AP.Business.Model.Registration.Events
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.EventSourcing;

    public class AnchorAssignmentsCreated : VersionedEvent
    {
        public class AnchorAssignmentInfo
        {
            public int Position { get; set; }
            public Guid SeatType { get; set; }
        }

        public Guid OrderId { get; set; }
        public IEnumerable<AnchorAssignmentInfo> Anchors { get; set; }
    }
}
