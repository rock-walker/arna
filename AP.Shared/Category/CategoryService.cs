using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using AP.Repository.Common;
using System.Linq;
using AP.ViewModel.Mapper;

namespace AP.Shared.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;

        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categories = categoryRepo;
        }

        public async Task<IEnumerable<MenuViewModel>> GetHierarchical()
        {
            var allMenu = await _categories.GetHierarchical();
            return allMenu.Select(x => x.MapTo());

        }

        public async Task<IEnumerable<MenuViewModel>> GetTopLevel()
        {
            var topLevelMenu = await _categories.GetTopLevel();
            return topLevelMenu.Select(x => x.MapTo());
        }
    }
}
