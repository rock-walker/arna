using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.Model.Common;

namespace AP.Repository.Common.Contracts
{
    //TODO: DataContract is temp solution; add repository Dto
    public interface ICategoryRepository
    {
        Task<IEnumerable<CategoryModel>> GetHierarchical();
        Task<IEnumerable<CategoryModel>> GetTopLevel();
    }
}
