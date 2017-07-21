using AP.DataContract;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AP.Application.Controllers
{
    public class CalendarController : Controller
    {
	    private readonly IReserve _calendar;

	    public CalendarController(IReserve calendar)
	    {
		    _calendar = calendar;
	    }

	    public async Task<DaySettings> GetCalendarSettings()
	    {
		    return await _calendar.GetDaySettings();
	    }

		//[Route("serviceType/{srvType}/serviceId/{srvId:int}")]
        /*
	    public async Task<ICollection<Reservation>> GetTodayReservations(short srvType, int srvId)
	    {
		    return await _calendar.GetDayReservations(srvType, srvId, DateTime.Today);
	    }
        */
    }
}
