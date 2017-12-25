using AP.Business.Model.Enums;
using AP.ViewModel.Workshop;
using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Attendee
{
    public class AttendeeAutoViewModel
    {
        public AutoGearType GearType { get; set; }
        public int ManufacturYear { get; set; }

        [Required]
        [StringLength(17)]
        public string Vin { get; set; }
        public AutoBodyType BodyType { get; set; }
        public AutoOptions Options { get; set; }
        public float EngineVolume { get; set; }
        public AutoFuelType FuelType { get; set; }

        public AutobrandViewModel AutoBrand { get; set; }
    }
}
