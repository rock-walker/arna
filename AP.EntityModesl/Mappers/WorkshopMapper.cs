using AP.ViewModel.Workshop;
using System.Linq;

namespace AP.EntityModel.Mappers
{
    public static class WorkshopMapper
    {
        public static WorkshopViewModel MapTo(this AutoDomain.Workshop data)
        {
            return new WorkshopViewModel
            {
                Id = data.ID,
                Name = data.Name,
                Categories = data.WorkshopCategories.Select(x => x.Category.MapTo()),
                Contacts = data.Contact.MapTo(),
                Address = data.Address.MapTo(),
                Location = data.Location.MapTo()
            };
        }
    }
}
