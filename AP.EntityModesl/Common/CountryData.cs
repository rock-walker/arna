using System;
using System.ComponentModel.DataAnnotations;

namespace AP.EntityModel.Common
{
    public class CountryData
    {
        public Guid ID { get; set; }

        [StringLength(30)]
        public string Fullname { get; set; }

        [StringLength(3)]
        public string Iso3Name { get; set; }

        [StringLength(2)]
        public string Shortname { get; set;}

        public int PhoneCode { get; set; }
    }
}
