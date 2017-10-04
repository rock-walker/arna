using AP.EntityModel.Common;
using AP.Business.Model.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.AutoDomain
{
    public class WorkshopCategoryData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public Guid WorkshopID { get; set; }
        public int CategoryID { get; set; }

        public WorkshopData Workshop { get; set; }
        public CategoryData Category { get; set; }
    }
}
