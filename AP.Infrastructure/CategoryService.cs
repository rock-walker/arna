using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using AP.Repository.Common;

namespace AP.Shared.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _category;
        public async Task<IEnumerable<MenuViewModel>> GetHierarchical()
        {
            return await _category.GetHierarchical();
        }

        public async Task<IEnumerable<MenuViewModel>> GetTopLevel()
        {
            return await _category.GetTopLevel();
        }
    }
}
