using System;
using System.Collections.Generic;
using System.Text;

namespace AP.EntityModel.AutoDomain
{
    public class WorkshopAutoBrand
    {
        public Guid ID { get; set; }
        public Guid WorkshopID { get; set; }
        public int AutoBrandID { get; set; }

        public Workshop Workshop { get; set; }
        public AutoBrand AutoBrand { get; set; }
    }
}
