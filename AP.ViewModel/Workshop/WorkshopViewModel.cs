using System;
using System.Collections.Generic;
using AP.ViewModel.Common;

namespace AP.ViewModel.Workshop
{
    public class WorkshopViewModel
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public int AnchorNumber { get; set; }

        public decimal PayHour { get; set; }

        public IEnumerable<WorkshopCategoryViewModel> WorkshopCategories { get; set; }
        public ContactViewModel Contact { get; set; }
        public AddressViewModel Address { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
