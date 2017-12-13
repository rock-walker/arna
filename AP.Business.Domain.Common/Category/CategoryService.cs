using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;
using System.Linq;
using AP.ViewModel.Mapper;
using AP.Repository.Common.Contracts;

namespace AP.Business.Domain.Common.Category
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
            var fullMenu = await _categories.GetHierarchical();
            return fullMenu.Select(x => x.MapTo());

        }

        public async Task<IEnumerable<MenuViewModel>> GetTopLevel()
        {
            var topLevelMenu = await _categories.GetTopLevel();
            return topLevelMenu.Select(x => x.MapTo());
        }
    }
}
