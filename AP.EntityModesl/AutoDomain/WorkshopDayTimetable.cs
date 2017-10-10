using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace AP.EntityModel.AutoDomain
{
    public class WorkshopDayTimetableData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public Guid WorkshopID { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan? DinnerStart { get; set; }
        public TimeSpan? DinnerEnd { get; set; }
        public DayOfWeek Day { get; set; }
        public bool IsHoliday { get; set; }

        public WorkshopData Workshop { get; set; }
    }
}
