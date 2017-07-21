using AP.EntityModel.Common;
using AP.ViewModel.Common;

namespace AP.EntityModel.Mappers
{
    public static class CityMapper
    {
        public static CityViewModel MapTo(this City data)
        {
            return new CityViewModel
            {
                GoogleCode = data.GoogleCode,
                Name = data.Name,
                Ru = data.Ru
            };
        }
    }
}
