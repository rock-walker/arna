using System;

namespace AP.EntityModel.AutoDomain
{
    public class WorkshopAutoBrand
    {
        public Guid ID { get; set; }
        public Guid WorkshopID { get; set; }
        public int AutoBrandID { get; set; }

        public WorkshopData Workshop { get; set; }
        public AutoBrandData AutoBrand { get; set; }
    }
}
