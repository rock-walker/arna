using AP.Business.Model.Enums;
using AP.EntityModel.AutoDomain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Repository.Common.Contracts
{
    public interface IAutobrandRepository
    {
        Task<IEnumerable<AutoBrandData>> GetAutoBrands(CarClassification autoType);
    }
}
