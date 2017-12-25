namespace AP.Business.Model.Registration.Events
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.EventSourcing;

    public class OrderPartiallyReserved : VersionedEvent
    {
        public DateTime ReservationExpiration { get; set; }

        public IEnumerable<AnchorQuantity> Anchors { get; set; }
    }
}
