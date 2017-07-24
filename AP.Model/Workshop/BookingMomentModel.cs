using AP.Business.Model.Common;
using AP.Model.Enums;
using System;

namespace AP.Business.Model.Workshop
{
    public class BookingMomentModel
    {
        public Guid WorkshopId { get; set; }
        public CategoryModel Category { get; set; }
        public WorkshopStatus MomentStatus { get; set; }
    }
}
