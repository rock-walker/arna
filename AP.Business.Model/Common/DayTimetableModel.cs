using System;

namespace AP.Business.Model.Common
{
    public class DayTimetableModel
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan? DinnerStart { get; set; }
        public TimeSpan? DinnerEnd { get; set; }
        public bool IsHoliday { get; set; }
    }
}
