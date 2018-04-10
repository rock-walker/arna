using System.Collections.Generic;
using AP.Business.Model.Enums;
using AP.ViewModel.Workshop;
using EntityFramework.DbContextScope.Interfaces;
using AP.Repository.Common.Contracts;
using AutoMapper;

namespace AP.Business.Domain.Common
{
    public class AutobrandService : IAutobrandService
    {
        private readonly IDbContextScopeFactory dbContextScope;
        private readonly IAutobrandRepository autobrandRepository;

        public AutobrandService(IDbContextScopeFactory scope, IAutobrandRepository repo)
        {
            dbContextScope = scope;
            autobrandRepository = repo;
        }

        public IEnumerable<AutobrandViewModel> Get(CarClassification autoType)
        {
            using (var dbContext = dbContextScope.CreateReadOnly())
            {
                var autoBrandsData = autobrandRepository.GetAutoBrands(autoType);
                var output = Mapper.Map<IEnumerable<AutobrandViewModel>>(autoBrandsData);

                return output;
            }
        }
    }
}
