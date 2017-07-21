using AP.ViewModel.Workshop;
using System;

namespace AP.ViewModel
{
    public class Booking
    {
        public Guid Id { get; set; }
        public WorkshopViewModel Status { get; set; }
        public DateTime BookingMoment { get; set; }
    }
}
