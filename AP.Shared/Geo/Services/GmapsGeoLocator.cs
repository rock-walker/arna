using System;
using AP.Shared.Geo.Contracts;
using System.Xml.Linq;
using System.Linq;
using AP.Shared.Geo.Models;
using AP.Core.Extensions;

namespace AP.Shared.Geo.Services
{
    public class GMapsGeoLocator : IGeoLocator
    {
        private string apiKey = "";
        private string googleMapsUrl = @"https://maps.googleapis.com/maps/api/place/details/xml?placeid={0}&key={1}";

        public GooglePlacesResult DecodeAddress(string placeId)
        {
            string apiKey = ""; // Your api key
            string url = string.Format(googleMapsUrl, placeId, apiKey);
            XDocument doc = XDocument.Load(url);
            var businessResults = doc.Root.Element("result");

            var addressComponents = businessResults.Elements("address_component").Where(e => e.Element("type") != null);
            var street = businessResults.Element("vicinity").Value;
            var cityTypes = new[] { "locality", "sublocality", "sublocality_level_1", "sublocality_level_2", "sublocality_level_3", "sublocality_level_4", "sublocality_level_5" };
            var stateTypes = new[] { "administrative_area_level_1" };
            var countryTypes = new[] { "country" };
            var zipTypes = new[] { "postal_code" };
            var location = businessResults.Element("geometry").Element("location");
            var reviews = businessResults.Elements("review").Select(r => new GooglePlacesReview
            {
                ReviewDate = FromUnixTime(r.Element("time").Value.TryParseValue<Int64>()),
                ReviewText = r.Element("text").Value,
                AuthorName = r.Element("author_name").Value,
                Rating = r.Element("rating").Value.TryParseValue<Decimal>(),
                Language = r.Element("language").Value
            });
            var openingHours = businessResults.Element("opening_hours");

            return new GooglePlacesResult
            {
                Name = businessResults.Element("name").Value,
                Address = street.Remove(street.LastIndexOf(",")),
                PhoneNumber = businessResults.Element("formatted_phone_number").Value,
                City = addressComponents.FirstOrDefault(e => cityTypes.Any(s => s == e.Element("type").Value)).Element("long_name").Value,
                State = addressComponents.FirstOrDefault(e => stateTypes.Any(s => s == e.Element("type").Value)).Element("long_name").Value,
                Country = addressComponents.FirstOrDefault(e => countryTypes.Any(s => s == e.Element("type").Value)).Element("long_name").Value,
                Zip = addressComponents.FirstOrDefault(e => zipTypes.Any(s => s == e.Element("type").Value)).Element("long_name").Value,
                Latitude = location.Element("lat").Value.TryParseValue<Decimal>(),
                Longitude = location.Element("lng").Value.TryParseValue<Decimal>(),
                Rating = businessResults.Element("rating").Value.TryParseValue<Decimal>(),
                Url = businessResults.Element("url").Value,
                InternationalPhoneNumber = businessResults.Element("international_phone_number").Value,
                Reviews = reviews,
                OpenNow = openingHours.Element("open_now").Value.TryParseValue<bool>(),
                OpenHours = openingHours.Elements("weekday_text").Select(e => e.Value),
                UtcOffset = businessResults.Element("utc_offset").Value.TryParseValue<Int32>(),
                UserRatingsTotal = businessResults.Element("user_ratings_total").Value.TryParseValue<Int32>()
            };
        }

        private DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
    }
}
