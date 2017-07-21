using AP.Model.Enums;
using AP.ViewModel.Common;
using System;

namespace AP.ViewModel.Workshop
{
    public class BookingMomentViewModel
    {
        public Guid WorkshopId { get; set; }
        public MenuViewModel Category { get; set; }
        public WorkshopStatus MomentStatus { get; set; }
    }
}
