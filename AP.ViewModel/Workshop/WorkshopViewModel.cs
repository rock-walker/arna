using System;
using System.Collections.Generic;
using AP.ViewModel.Common;
using AP.Business.Model.Common;

namespace AP.ViewModel.Workshop
{
    public class WorkshopViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public IEnumerable<CategoryModel> Categories { get; set; }
        public ContactViewModel Contacts { get; set; }
        public AddressViewModel Address { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
