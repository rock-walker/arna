using AP.ViewModel.Common;
using AP.EntityModel.Common;
using System.Linq;

namespace AP.EntityModel.Mappers
{
    public static class CategoryMapper
    {
        public static MenuViewModel MapTo(this Category source)
        {
            return new MenuViewModel
            {
                Id = source.Id,
                Parent = source.Parent,
                Sub = source.SubCategories != null
                    ? source.SubCategories.Select(MapTo)
                    : null,
                Title = source.Title,
                Link = source.Link
            };
        }
    }
}
