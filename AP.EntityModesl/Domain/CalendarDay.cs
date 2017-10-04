using AP.EntityModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Domain
{
	[Table("CalendarDay")]
	public class CalendarDay
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid Id { get; set; }

		[Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime UtcDate { get; set; }

		public virtual ICollection<Reservation> Reservations { get; set; }
		public virtual CategoryData Category { get; set; }
	}
}
