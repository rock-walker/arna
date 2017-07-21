using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AP.Business.AutoDomain.Workshop.Contracts
{
    public interface IWorkshopBookingService
    {
        Task<IEnumerable<BookingMomentViewModel>> GeMomentBookings(IEnumerable<Guid> workshops);
    }
}
