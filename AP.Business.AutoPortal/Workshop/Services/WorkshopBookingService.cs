using System;
using System.Threading.Tasks;
using AP.ViewModel;
using AP.Repository.Workshop.Contracts;
using AP.Business.AutoDomain.Workshop.Contracts;
using System.Collections.Generic;
using AP.ViewModel.Workshop;

namespace AP.Business.AutoDomain.Workshop.Services
{
    public class WorkshopBookingService : IWorkshopBookingService
    {
        private readonly IWorkshopBookingRepository _repo;
        public WorkshopBookingService(IWorkshopBookingRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<BookingMomentViewModel>> GeMomentBookings(IEnumerable<Guid> workshops)
        {
            return await _repo.GetMomentBookings(workshops);
        }
    }
}
