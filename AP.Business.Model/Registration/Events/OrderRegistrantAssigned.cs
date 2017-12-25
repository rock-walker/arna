namespace AP.Business.Model.Registration.Events
{
    using Infrastructure.EventSourcing;
    using System;
    using System.Collections.Generic;

    public class OrderRegistrantAssigned : VersionedEvent
    {
        public IEnumerable<int> CategoryIds { get; set; }
        public string Description { get; set; }
        public DateTime? BookingTime { get; set; }
        public Guid AttendeeID { get; set; }
        public Guid AutoID { get; set; }
    }
}
