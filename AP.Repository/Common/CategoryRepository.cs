using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using AP.Repository.Infrastructure;
using AP.Repository.Context;
using AP.EntityModel.Common;
using AP.EntityModel.Mappers;

namespace AP.Repository.Common
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly GeneralContext _ctx;

        public CategoryRepository(GeneralContext context)
        {
            _ctx = context;
        }

        public async Task<IEnumerable<MenuViewModel>> GetHierarchical()
        {
            var menu = await Task.Run(() =>
            {
                var categories = _ctx.Categories.ToList();
                if (!categories.Any())
                {
                    //throw Exception here
                    ;
                }

                var builtCategories = MenuBuilder.BuildCategoriesHierarchy(categories.OfType<Category>(), 0);
                return builtCategories.Select(x => x.MapTo());
            });

            return menu;
        }

        public async Task<IEnumerable<MenuViewModel>> GetTopLevel()
        {
            var result = await Task.Run(() =>
            {
                var categories = _ctx.Categories.ToList();
                return categories.Select(x => x.MapTo());
            });

            return result;
        }

        
    }
}
