using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel;

namespace AP.Business.AutoDomain.Calendar
{
	public interface ICalendarService
	{
		Task<ICollection<BookingViewModel>> GetDayReservations(short serviceType, int serviceId, DateTime dt);
		Task<DaySettings> GetDaySettings();
	}
}
