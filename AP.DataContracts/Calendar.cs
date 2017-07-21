using AP.DataContract;
using System;
using System.Collections.Generic;

namespace AP.ViewModel
{
	public class Scheduler
	{
		public IEnumerable<DateTime> DefaultDayOffs;
		public IEnumerable<DateTime> CustomDayOffs;
	}

	public class DaySettings
	{
		public TimeSpan StartWorkTime;
		public TimeSpan EndWorkTime;
		public int DefaultInterruption;
	}

	public class CalendarDay
	{
		public IEnumerable<BookingViewModel> WorkingRanges;
		public IEnumerable<BookingViewModel> CustomInterruptions;
		public IEnumerable<Specialist> AvailableSpecialists;
	}

	public class BookingViewModel
	{
		public int PersonCount;
		public int DefaultRange;
		public int CustomRange;
		public TimeRangeStatus Status;
		public TimeRangeType Type;
	}

	public enum TimeRangeStatus
	{
		Free,
		Reserved,
		Tentative,
        Cancelled
	}

	public enum TimeRangeType
	{
		Working,
		Interruption
	}
}
