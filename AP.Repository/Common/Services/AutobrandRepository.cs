using AP.Repository.Common.Contracts;
using System.Collections.Generic;
using AP.Business.Model.Enums;
using AP.EntityModel.AutoDomain;
using AP.Repository.Context;
using EntityFramework.DbContextScope.Interfaces;
using System.Linq;
using AP.Core.Database;

namespace AP.Repository.Common.Services
{
    public class AutobrandRepository : AmbientContext<GeneralContext>, IAutobrandRepository
    {
        public AutobrandRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public IEnumerable<AutoBrandData> GetAutoBrands(CarClassification autoType)
        {
            return DbContext
                    .Autobrands
                    .Where(x => (x.AutoClassification & autoType) == autoType);
        }
    }
}
