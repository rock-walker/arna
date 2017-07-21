using AP.EntityModel.Common;
using System;
using System.Collections.Generic;
using static AP.EntityModel.Common.DomainModels;

namespace AP.EntityModel.AutoDomain
{
    public class Workshop
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string BrandName { get; set; }
        public Guid ContactID { get; set; }
        public Guid AddressID { get; set; }
        public Guid LocationID { get; set; }
        public Guid? LogoID { get; set; }
        public int AutoBrandID { get; set; }

        public AutoBrand AutoBrand { get; set; }
        public Contact Contact { get; set; }
        public Address Address { get; set; }
        public GeoMarker Location { get; set; }
        public AvatarImage Logo { get; set; }
        public ICollection<WorkshopCategory> WorkshopCategories { get; set; }
    }
}
