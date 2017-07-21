using System;

namespace AP.EntityModel.Common
{
    public class Address
    {
        public Guid ID { get; set; }
        public Guid CityID { get; set; }
        public string Country { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string SubBuilding { get; set; }
        public int? Apartment { get; set; }
        public string SubApt { get; set; }
        public string Comments { get; set; }

        public City City { get; set; }
    }
}
