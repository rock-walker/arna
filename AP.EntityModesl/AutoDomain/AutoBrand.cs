using AP.EntityModel.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace AP.EntityModel.AutoDomain
{
    public class AutoBrand
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }
        public Guid CountryID { get; set; }

        public Country Country { get; set; }
    }
}
