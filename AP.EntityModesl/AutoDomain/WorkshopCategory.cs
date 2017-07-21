using AP.EntityModel.Common;
using AP.Model.Enums;
using System;

namespace AP.EntityModel.AutoDomain
{
    public class WorkshopCategory
    {
        public Guid ID { get; set; }
        public Guid WorkshopID { get; set; }
        public int CategoryID { get; set; }
        public WorkshopStatus MomentBookingState { get; set; }

        public Workshop WorkshopData { get; set; }
        public Category Category { get; set; }
    }
}
