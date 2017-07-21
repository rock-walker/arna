using AP.EntityModel.Common;
using AP.ViewModel.Common;

namespace AP.EntityModel.Mappers
{
    public static class AddressMapper
    {
        public static AddressViewModel MapTo(this Address data)
        {
            return new AddressViewModel
            {
                Street = data.Street,
                Building = data.Building,
                City = data.City.MapTo()
            };
        }
    }
}
