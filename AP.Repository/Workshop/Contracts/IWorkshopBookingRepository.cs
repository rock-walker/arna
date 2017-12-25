using AP.Business.Model.Workshop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopBookingRepository
    {
        Task<IEnumerable<BookingMomentModel>> GetMomentBookings(IEnumerable<string> workshops);
    }
}
