using System.Collections.Generic;
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

        public IEnumerable<CategoryViewModel> Get(IEnumerable<int> ids)
        {
            return _categories.Get(ids).Select(x => x.MapTo());
        }

        public IEnumerable<CategoryViewModel> GetHierarchical()
        {
            var fullMenu = _categories.GetHierarchical();
            return fullMenu.Select(x => x.MapTo());
        }

        public IEnumerable<CategoryViewModel> GetTopLevel()
        {
            var topLevelMenu = _categories.GetTopLevel();
            return topLevelMenu.Select(x => x.MapTo());
        }
    }
}
