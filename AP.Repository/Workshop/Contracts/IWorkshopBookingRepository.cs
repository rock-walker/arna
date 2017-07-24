using AP.Business.Model.Workshop;
using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopBookingRepository
    {
        Task<IEnumerable<BookingMomentModel>> GetMomentBookings(IEnumerable<Guid> workshops);
    }
}
