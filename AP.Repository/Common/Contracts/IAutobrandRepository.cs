using AP.Business.Model.Enums;
using AP.EntityModel.AutoDomain;
using System.Collections.Generic;

namespace AP.Repository.Common.Contracts
{
    public interface IAutobrandRepository
    {
        IEnumerable<AutoBrandData> GetAutoBrands(CarClassification autoType);
    }
}
