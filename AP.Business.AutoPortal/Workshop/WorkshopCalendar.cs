using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.AutoDomain.Calendar;
using AP.ViewModel;

namespace AP.Business.AutoDomain.Workshop
{
	public class WorkshopCalendar : ICalendarService
	{
		public Task<ICollection<BookingViewModel>> GetDayReservations(short serviceType, int serviceId, DateTime dt)
		{
            throw new NotImplementedException();
		}

		public Task<DaySettings> GetDaySettings()
		{
			throw new NotImplementedException();
		}
    }
}
