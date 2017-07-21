using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Server.Application;
using AP.ViewModel.Workshop;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP.Server.Controllers
{
    [Route("api/[controller]")]
    public class WorkshopBookingController : Controller
    {
        private readonly IWorkshopBookingService _bookingService;

        public WorkshopBookingController(IWorkshopBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [Route("momentBookings")]
        public async Task<IEnumerable<BookingMomentViewModel>> GetMomentBookings (
            [ModelBinder(BinderType = typeof(CommaDelimitedArrayModelBinder))]
            IEnumerable<Guid> workshops)
        {
            if (!workshops.Any())
            {
                throw new Exception("Empty list of workshops");
            }
            return await _bookingService.GeMomentBookings(workshops);
        }
    }
}
