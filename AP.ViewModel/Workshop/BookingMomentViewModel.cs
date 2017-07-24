using System.Collections.Generic;

namespace AP.ViewModel.Workshop
{
    public class BookingMomentViewModel
    {
        public WorkshopViewModel Workshop { get; set; }
        public IEnumerable<CategoryBookingViewModel> BookingMoments { get; set; }
    }
}
