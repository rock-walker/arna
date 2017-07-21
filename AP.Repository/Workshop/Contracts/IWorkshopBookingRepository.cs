using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopBookingRepository
    {
        Task<IEnumerable<BookingMomentViewModel>> GetMomentBookings(IEnumerable<Guid> workshops);
    }
}
