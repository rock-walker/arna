using AP.Business.Model.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AP.EntityModel.AutoDomain
{
    public class WorkshopCategoryData
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }

        public Guid WorkshopID { get; set; }
        public int CategoryID { get; set; }

        [NotMapped]
        public CategoryModel Category { get; set; }
        //TODO: investigate how to skip filling this property from DB
        public WorkshopData Workshop { get; set; }
    }
}
