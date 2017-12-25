using AP.Business.Model.Enums;
using AP.EntityModel.AutoDomain;
using System;
using System.ComponentModel.DataAnnotations;

namespace AP.EntityModel.AttendeeDomain
{
    public class AttendeeAutoData
    {
        public Guid ID { get; set; }
        public AutoGearType GearType { get; set; }
        public int ManufacturYear { get; set; }
        [StringLength(17)]
        public string Vin { get; set; }
        public AutoBodyType BodyType { get; set; }
        public AutoOptions Options { get; set; }
        public float EngineVolume { get; set; }
        public AutoFuelType FuelType { get; set; }

        public AttendeeData Attendee { get; set; }
        public int AutoBrandID { get; set; }
    }
}
