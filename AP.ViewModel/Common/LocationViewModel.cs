using System.ComponentModel.DataAnnotations;

namespace AP.ViewModel.Common
{
    public class LocationViewModel
    {
        [Range(0.0001, 180)]
        public double Lng { get; set; }

        [Range(0.0001, 180)]
        public double Lat { get; set; }
    }
}
