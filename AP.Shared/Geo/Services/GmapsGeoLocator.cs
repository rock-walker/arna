using AP.Shared.Geo.Contracts;
using System.Xml.Linq;
using System.Linq;
using AP.Shared.Geo.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AP.Shared.Geo.Services
{
    public class GMapsGeoLocator : IGeoLocator
    {
        private const string apiKey = "AIzaSyClRth8aBUuLmAuJwps4jRnBMX49hJ_tCU";
        private const string googleMapsUrl = @"https://maps.googleapis.com/maps/api/place/details/json?placeid={0}&key={1}&language=ru";

        private string[] gCityTypes = new[] { "locality", "sublocality", "sublocality_level_1", "sublocality_level_2", "sublocality_level_3", "sublocality_level_4", "sublocality_level_5" };
        private const string gCountryType = "country";
        private const string gStreetType = "route";
        private const string gStreetNumber = "street_number";

        public async Task<GooglePlacesResult> DecodeAddress(string placeId)
        {
            if (string.IsNullOrEmpty(placeId))
            {
                return null;
            }

            string url = string.Format(googleMapsUrl, placeId, apiKey);
            GMapObject jsonData = null;
            using (var client = new HttpClient())
            {
                var data = await client.GetStringAsync(url);
                jsonData = JsonConvert.DeserializeObject<GMapObject>(data);
            }

            var doc = new XDocument();
            var businessResults = jsonData.result;

            var addressComponents = businessResults.address_components.Where(e => e.types != null);

            var res = new GooglePlacesResult
            {
                Name = businessResults.name,
                Street = RetrieveStreetName(addressComponents),
                Number = RetrieveStreetNumber(addressComponents),
                City = RetrieveCity(addressComponents),
                Country = RetrieveCountry(addressComponents)
            };

            return res;
        }

        private string RetrieveStreetNumber(IEnumerable<AddressComponent> addressComponents)
        {
            var gNumberComponent = addressComponents.FirstOrDefault(e => e.types.First() == gStreetNumber);
            return gNumberComponent?.long_name;
        }

        private string RetrieveStreetName(IEnumerable<AddressComponent> addressComponents)
        {
            var gStreetComponent = addressComponents.FirstOrDefault(e => e.types.First() == gStreetType);
            return gStreetComponent?.long_name;
        }

        private string RetrieveCity(IEnumerable<AddressComponent> addressComponents)
        {
            var gCityComponent = addressComponents.FirstOrDefault(e => gCityTypes.First() == e.types.First());
            return gCityComponent?.long_name;
        }

        private string RetrieveCountry(IEnumerable<AddressComponent> addressComponents)
        {
            var gCountryComponent = addressComponents.FirstOrDefault(e => e.types.First() == gCountryType);
            return gCountryComponent?.short_name;
        }
    }
}
