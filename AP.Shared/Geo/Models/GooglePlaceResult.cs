using System.Collections.Generic;

namespace AP.Shared.Geo.Models
{
    public class GooglePlacesResult
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Rating { get; set; }
        public string Url { get; set; }
        public string InternationalPhoneNumber { get; set; }
        public IEnumerable<GooglePlacesReview> Reviews { get; set; }
        public bool OpenNow { get; set; }
        public IEnumerable<string> OpenHours { get; set; }
        public int UtcOffset { get; set; }
        public int UserRatingsTotal { get; set; }
    }
}
