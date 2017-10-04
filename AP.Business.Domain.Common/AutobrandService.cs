using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.Model.Enums;
using AP.ViewModel.Workshop;
using EntityFramework.DbContextScope.Interfaces;
using AP.Repository.Common.Contracts;
using AutoMapper;

namespace AP.Business.Domain.Common
{
    public class AutobrandService : IAutobrandService
    {
        private readonly IDbContextScopeFactory _dbContextScope;
        private readonly IAutobrandRepository _autobrandRepository;

        public AutobrandService(IDbContextScopeFactory scope, IAutobrandRepository repo)
        {
            _dbContextScope = scope;
            _autobrandRepository = repo;
        }

        public async Task<IEnumerable<AutobrandViewModel>> Get(CarClassification autoType)
        {
            using (var dbContext = _dbContextScope.CreateReadOnly())
            {
                var autoBrandsData = await _autobrandRepository.GetAutoBrands(autoType);
                var output = Mapper.Map<IEnumerable<AutobrandViewModel>>(autoBrandsData);

                return output;
            }
        }
    }
}
