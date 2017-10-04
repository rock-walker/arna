using AP.Business.Model.Workshop;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using AP.Business.Model.Enums;

namespace AP.EntityModel.Common
{
    public class CategoryData : DomainModels.BaseCategory, ICarClassification
    {
        public string Title { get; set; }
        public string Link { get; set; }

        public CarClassification? AutoClassification { get; set; }

        [NotMapped]
        public IEnumerable<CategoryData> SubCategories { get; set; }
    }
}