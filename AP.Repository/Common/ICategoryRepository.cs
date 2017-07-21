using System.Collections.Generic;
using System.Threading.Tasks;
using AP.ViewModel.Common;

namespace AP.Repository.Common
{
    //TODO: DataContract is temp solution; add repository Dto
    public interface ICategoryRepository
    {
        Task<IEnumerable<MenuViewModel>> GetHierarchical();
        Task<IEnumerable<MenuViewModel>> GetTopLevel();
    }
}
