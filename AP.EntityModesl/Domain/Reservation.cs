using System;
using EL.EntityModel;

namespace AP.EntityModel.Domain
{
	public class Reservation
	{
		public Guid Id { get; set; }
		public short StartH { get; set; }
		public short StartM { get; set; }
		public short EndH { get; set; }
		public short EndM { get; set; }
		public short Status { get; set; }
        public UserProfile Customer { get; set; }
	}
}
