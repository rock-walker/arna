using System;
using System.Threading.Tasks;
using AP.Repository.Workshop.Contracts;
using AP.Business.AutoDomain.Workshop.Contracts;
using System.Collections.Generic;
using AP.ViewModel.Workshop;
using System.Linq;

namespace AP.Business.AutoDomain.Workshop.Services
{
    public class WorkshopBookingService : IWorkshopBookingService
    {
        private readonly IWorkshopBookingRepository _repo;
        private readonly IWorkshopService _workshopService;
        public WorkshopBookingService(IWorkshopBookingRepository repo,
            IWorkshopService workshopService)
        {
            _repo = repo;
            _workshopService = workshopService;
        }

        public async Task<IEnumerable<BookingMomentViewModel>> GeMomentBookings(IEnumerable<Guid> workshops)
        {
            var workshopsData = await _workshopService.GetById(workshops);
            var bookings = await _repo.GetMomentBookings(workshops);
            
            //That works really slower, than upper variant and save us from async db context access
            //await Task.WhenAll(workshopsData, bookings);

            var workshopBookings = bookings.GroupBy(x => x.WorkshopId, 
                x => new CategoryBookingViewModel {
                    Category = x.Category.Id,
                    MomentStatus = x.MomentStatus
                },
                (key, elements) => new BookingMomentViewModel
                {
                    BookingMoments = elements,
                    Workshop = workshopsData.Single(x => x.ID == key)
                });

            return workshopBookings;
        }
    }
}
