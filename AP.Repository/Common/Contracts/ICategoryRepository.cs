using System.Collections.Generic;
using AP.Business.Model.Common;

namespace AP.Repository.Common.Contracts
{
    //TODO: DataContract is temp solution; add repository Dto
    public interface ICategoryRepository
    {
        IEnumerable<CategoryModel> GetHierarchical();
        IEnumerable<CategoryModel> GetTopLevel();
        IEnumerable<CategoryModel> Get(IEnumerable<int> ids);
    }
}
