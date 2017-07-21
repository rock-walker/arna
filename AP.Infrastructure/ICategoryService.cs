using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;

namespace AP.Shared.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<MenuViewModel>> GetHierarchical();
        Task<IEnumerable<MenuViewModel>> GetTopLevel();
    }
}
