using AP.Model.Enums;
using AP.ViewModel.Common;
using System;
using System.Collections.Generic;

namespace AP.ViewModel.Workshop
{
    public class BookingMomentViewModel
    {
        public WorkshopViewModel Workshop { get; set; }
        public IEnumerable<CategoryBookingViewModel> BookingMoments { get; set; }
    }
}
