using AP.ViewModel.Workshop;
using System.Linq;

namespace AP.EntityModel.Mappers
{
    public static class WorkshopMapper
    {
        public static WorkshopViewModel MapTo(this AutoDomain.WorkshopData data)
        {
            return new WorkshopViewModel
            {
                ID = data.ID,
                Name = data.Name,
                WorkshopCategories = data.WorkshopCategories.Select(x => x.Category.MapFrom()).ToList(),
                Contact = data.Contact.MapTo(),
                Address = data.Address.MapTo(),
                Location = data.Location.MapTo()
            };
        }

        public static WorkshopShortViewModel MapToShort(this AutoDomain.WorkshopData data)
        {
            return new WorkshopShortViewModel
            {
                Id = data.ID,
                Name = data.Name,
                Location = data.Location.MapTo()
            };
        }
    }
}
