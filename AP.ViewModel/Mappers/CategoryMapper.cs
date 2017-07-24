using AP.Business.Model.Common;
using AP.ViewModel.Common;
using System.Linq;

namespace AP.ViewModel.Mapper
{
    public static class CategoryMapper
    {
        public static MenuViewModel MapTo(this CategoryModel model)
        {
            return new MenuViewModel
            {
                Id = model.Id,
                Link = model.Link,
                Parent = model.Parent,
                Title = model.Title,
                Sub = model.Sub != null
                        ? model.Sub.Select(x => x.MapTo())
                        : null
            };
        }
    }
}
