using AP.Core.Model.User;
using AP.EntityModel.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static AP.EntityModel.Common.DomainModels;

namespace AP.EntityModel.AutoDomain
{
    public class Workshop
    {
        public Guid ID { get; set; }

        [Required]
        [StringLength(70)]
        public string Name { get; set; }

        [StringLength(20)]
        public string ShortName { get; set; }

        [StringLength(50)]
        public string BrandName { get; set; }
        public int AnchorNumber { get; set; }
        public decimal PayHour { get; set; }
        public float AvgRate { get; set; }

        public Guid? ContactID { get; set; }
        public Guid? AddressID { get; set; }
        public Guid LocationID { get; set; }
        public Guid? LogoID { get; set; }


        public AutoBrand AutoBrand { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
        public GeoMarker Location { get; set; }
        public AvatarImage Logo { get; set; }
        public ApplicationUser BasicUserAccount { get; set; }
        public ICollection<WorkshopCategory> WorkshopCategories { get; set; }
        public ICollection<WorkshopAutoBrand> AutoBrands { get; set; }
    }
}
