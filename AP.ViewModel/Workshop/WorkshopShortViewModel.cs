using AP.ViewModel.Common;
using System;

namespace AP.ViewModel.Workshop
{
    public class WorkshopShortViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
