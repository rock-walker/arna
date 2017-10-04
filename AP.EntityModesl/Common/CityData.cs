using System;
using System.ComponentModel.DataAnnotations;

namespace AP.EntityModel.Common
{
    public class CityData
    {
        public Guid ID { get; set; }

        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(30)]
        public string Ru { get; set; }

        public Guid? CountryID { get; set; }

        public virtual CountryData Country { get; set; }
    }
}
