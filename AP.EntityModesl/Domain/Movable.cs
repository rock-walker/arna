using AP.EntityModel.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Domain
{
	[Table("Movables")]
	public class Movable : DomainModels.Specialist
	{
		public virtual Details Details { get; set; }
		public virtual Gallery Gallery { get; set; }
		public virtual ICollection<CalendarDay> CalendarDays { get; set; }
	}
}
