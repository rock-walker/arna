using System.Collections.Generic;
using AP.Business.Model.Common;

namespace AP.Repository.Common.Contracts
{
    public interface ICategoryRepository
    {
        IEnumerable<CategoryModel> GetHierarchical(int root);
        IEnumerable<CategoryModel> GetTopLevel();
        IEnumerable<CategoryModel> Get(IEnumerable<int> ids);
    }
}
