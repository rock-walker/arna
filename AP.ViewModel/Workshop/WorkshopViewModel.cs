using System;
using System.Collections.Generic;
using AP.ViewModel.Common;
using AP.ViewModel.Booking;

namespace AP.ViewModel.Workshop
{
    public class WorkshopViewModel
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public int AnchorNumber { get; set; }

        public decimal PayHour { get; set; }

        public string Description { get; set; }
        public DateTime RegisterDate { get; set;  }

        public IEnumerable<WorkshopCategoryViewModel> WorkshopCategories { get; set; }
        public IEnumerable<AutobrandViewModel> WorkshopAutobrands { get; set; }
        public IEnumerable<DayTimetableViewModel> WorkshopWeekTimetable { get; set; }
        public IEnumerable<AnchorTypeViewModel> Anchors { get; set; }
        public ContactViewModel Contact { get; set; }
        public AddressViewModel Address { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
