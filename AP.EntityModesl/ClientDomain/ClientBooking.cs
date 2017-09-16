using AP.Business.Model.Enums;
using AP.Core.Model.User;
using AP.EntityModel.Common;
using System;
using System.Collections.Generic;

namespace AP.EntityModel.AutoDomain
{
    public class ClientBooking
    {
        public Guid ID { get; set; }
        public Guid UserID { get; set; }
        public Guid WorkshopID { get; set; }
        public long OrderTime { get; set; }
        public long ArrivalTime { get; set; }

        public ICollection<Category> Categories { get; set; }
        public string Description { get; set; }

        public BookingStatus BookingStatus { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public Workshop Workshop { get; set; }
    }
}
