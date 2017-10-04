using AP.EntityModel.Common;
using AP.ViewModel.Common;

namespace AP.EntityModel.Mappers
{
    public static class CityMapper
    {
        public static CityViewModel MapTo(this CityData data)
        {
            return new CityViewModel
            {
                Name = data.Name,
                Ru = data.Ru
            };
        }
    }
}
