using System;
using System.ComponentModel.DataAnnotations;

namespace AP.EntityModel.Common
{
    public class Address
    {
        public Guid ID { get; set; }

        public Guid? CityID { get; set; }
        [StringLength(50)]
        public string Street { get; set; }
        [StringLength(20)]
        public string Building { get; set; }
        [StringLength(10)]
        public string SubBuilding { get; set; }
        public int? Apartment { get; set; }
        [StringLength(10)]
        public string SubApt { get; set; }
        [StringLength(256)]
        public string Comments { get; set; }

        public virtual City City { get; set; }
    }
}
