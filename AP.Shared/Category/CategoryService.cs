using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using AP.Repository.Common;

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
            return await _categories.GetHierarchical();
        }

        public async Task<IEnumerable<MenuViewModel>> GetTopLevel()
        {
            return await _categories.GetTopLevel();
        }
    }
}
