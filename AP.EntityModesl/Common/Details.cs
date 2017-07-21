using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Common
{
	public class Details
	{
		public int Id { get; set; }
		public virtual Contact Contact { get; set; }
		public string Experience { get; set; }
	}
}
