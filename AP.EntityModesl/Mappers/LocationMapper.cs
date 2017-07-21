using AP.EntityModel.Common;
using AP.ViewModel.Common;

namespace AP.EntityModel.Mappers
{
    public static class LocationMapper
    {
        public static LocationViewModel MapTo(this GeoMarker data)
        {
            return new LocationViewModel
            {
                Lat = data.Lat,
                Lng = data.Lng
            };
        }
    }
}
