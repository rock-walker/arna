using System.Collections.Generic;
using System.Linq;
using AP.Repository.Infrastructure;
using AP.Repository.Context;
using AP.EntityModel.Common;
using AP.EntityModel.Mappers;
using AP.Business.Model.Common;
using AP.Repository.Common.Contracts;
using AP.Core.Database;
using EntityFramework.DbContextScope.Interfaces;

namespace AP.Repository.Common.Services
{
    public class CategoryRepository : AmbientContext<GeneralContext>, ICategoryRepository
    {
        public CategoryRepository(GeneralContext context, IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public IEnumerable<CategoryModel> Get(IEnumerable<int> ids)
        {
            var dbCategories = DbContext.Categories;
            var categories = new List<CategoryModel>();
            foreach(var id in ids)
            {
                var category = dbCategories.FirstOrDefault(x => x.Id == id);
                if (category != null)
                {
                    categories.Add(category.MapTo());
                }
            }

            return categories;
        }

        public IEnumerable<CategoryModel> GetHierarchical()
        {
            var categories = DbContext.Categories.ToList();
            if (!categories.Any())
            {
                //throw Exception here
                ;
            }

            var builtCategories = MenuBuilder.BuildCategoriesHierarchy(categories.OfType<CategoryData>(), 0);
            return builtCategories.Select(x => x.MapTo());
        }

        public IEnumerable<CategoryModel> GetTopLevel()
        {
            return DbContext.Categories.Select(x => x.MapTo());
        }
    }
}
