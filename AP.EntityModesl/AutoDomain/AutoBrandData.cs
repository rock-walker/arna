using AP.Business.Model.Workshop;
using AP.EntityModel.Common;
using System;
using System.ComponentModel.DataAnnotations;
using AP.Business.Model.Enums;

namespace AP.EntityModel.AutoDomain
{
    public class AutoBrandData : ICarClassification
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }
        public CarClassification? AutoClassification { get; set; }

        public Guid CountryID { get; set; }
        public CountryData Country { get; set; }
    }
}
