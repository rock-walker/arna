﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.Common
{
    public class AddressData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        [StringLength(32)]
        public string GooglePlaceId { get; set; }

        public virtual CityData City { get; set; }
    }
}
