using AP.Repository.Common.Contracts;
using System.Collections.Generic;
using AP.Business.Model.Enums;
using AP.EntityModel.AutoDomain;
using System.Threading.Tasks;
using AP.Repository.Base;
using AP.Repository.Context;
using EntityFramework.DbContextScope.Interfaces;
using System.Linq;

namespace AP.Repository.Common.Services
{
    public class AutobrandRepository : AmbientContext<GeneralContext>, IAutobrandRepository
    {
        public AutobrandRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public async Task<IEnumerable<AutoBrandData>> GetAutoBrands(CarClassification autoType)
        {
            return await Task.Run(() => 
                DbContext.Autobrands
                    .Where(x => (x.AutoClassification & autoType) == autoType));
        }
    }
}
