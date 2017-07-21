using AP.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AP.Business.AutoDomain.Calendar;

namespace AP.Application.Controllers
{
    public class CalendarController : Controller
    {
	    private readonly ICalendarService _calendar;

	    public CalendarController(ICalendarService calendar)
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
