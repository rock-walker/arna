using System;
using System.ComponentModel.DataAnnotations;

namespace AP.EntityModel.Common
{
    public class City
    {
        public Guid ID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }
        public string GoogleCode { get; set; }

        [StringLength(30)]
        public string Ru { get; set; }

        public Guid? CountryID { get; set; }

        public virtual Country Country { get; set; }
    }
}
