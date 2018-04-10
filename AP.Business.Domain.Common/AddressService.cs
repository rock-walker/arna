using System;
using EntityFramework.DbContextScope.Interfaces;
using AP.Repository.Common.Contracts;

namespace AP.Business.Domain.Common
{
    public class AddressService : IAddressService
    {
        private readonly IDbContextScopeFactory _dbContextScope;
        private readonly IAddressRepository _addressRepository;

        public AddressService(IAddressRepository addressRepository, IDbContextScopeFactory dbContextScope)
        {
            _addressRepository = addressRepository;
            _dbContextScope = dbContextScope;
        }

        public Guid? GetCityByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("City name is invalid");
            }

            using (var dbContext = _dbContextScope.CreateReadOnly())
            {
                return _addressRepository.GetCity(name);
            }
        }

        public Guid? AddCity(string cityName, string countryShort)
        {
            if (string.IsNullOrEmpty(cityName))
            {
                throw new ArgumentException("City name doesn't exist");
            }

            using (var scope = _dbContextScope.Create())
            {
                return _addressRepository.AddCity(cityName, countryShort);
            }
        }
    }
}
