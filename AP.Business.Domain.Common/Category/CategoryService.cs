using System.Collections.Generic;
using AP.ViewModel.Common;
using System.Linq;
using AP.ViewModel.Mapper;
using AP.Repository.Common.Contracts;
using EntityFramework.DbContextScope.Interfaces;

namespace AP.Business.Domain.Common.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;
        private readonly IDbContextScopeFactory scopeFactory;

        public CategoryService(ICategoryRepository categoryRepo,
            IDbContextScopeFactory scopeFactory)
        {
            _categories = categoryRepo;
            this.scopeFactory = scopeFactory;
        }

        public IEnumerable<CategoryViewModel> Get(IEnumerable<int> ids)
        {
            return _categories.Get(ids).Select(x => x.MapTo());
        }

        public IEnumerable<CategoryViewModel> GetHierarchical(int root)
        {
            using (var scope = scopeFactory.CreateReadOnly())
            {
                var fullMenu = _categories.GetHierarchical(root);

                return fullMenu.Select(x => x.MapTo());
            }
        }

        public IEnumerable<CategoryViewModel> GetTopLevel()
        {
            var topLevelMenu = _categories.GetTopLevel();
            return topLevelMenu.Select(x => x.MapTo());
        }
    }
}
