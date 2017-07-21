using System;

namespace AP.EntityModel.Common
{
	public class Contact
	{
		public Guid ID { get; set; }
		public string Mobile { get; set; } //divided by ';'
		public string Municipal { get; set; } //divided by ';'
		public string Email { get; set; }
		public string Chat { get; set; }
        public string Web { get; set; }
	}
}
