using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Common
{
    public class AddressViewModel
    {
        public string Street { get; set; }
        public string Building { get; set; }
        public int? Apartment { get; set; }
        public CityViewModel City { get; set; }

        [StringLength(30)]
        public string GooglePlaceId { get; set; }
    }
}
