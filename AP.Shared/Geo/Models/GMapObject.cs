using System.Collections.Generic;

namespace AP.Shared.Geo.Models
{
    public class GMapObject
    {
        public Result result { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public string name { get; set; }
        public string vicinity { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public IEnumerable<string> types { get; set; }
    }
}
