using System;

namespace AP.ViewModel.Booking
{
    public class DayTimetableViewModel
    {
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan? DinnerStart { get; set; }
        public TimeSpan? DinnerEnd { get; set; }
        public DayOfWeek Day { get; set; }
        public bool IsHoliday { get; set; }
    }
}
