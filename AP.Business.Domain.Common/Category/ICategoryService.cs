using System.Collections.Generic;
using AP.ViewModel.Common;

namespace AP.Business.Domain.Common.Category
{
    public interface ICategoryService
    {
        IEnumerable<CategoryViewModel> GetHierarchical(int root);
        IEnumerable<CategoryViewModel> GetTopLevel();
        IEnumerable<CategoryViewModel> Get(IEnumerable<int> ids);
    }
}
