using AP.Repository.Common.Contracts;
using System;
using AP.Repository.Context;
using EntityFramework.DbContextScope.Interfaces;
using System.Linq;
using AP.Core.Database;
using AP.EntityModel.Common;

namespace AP.Repository.Common.Services
{
    public class AddressRepository : AmbientContext<WorkshopContext>, IAddressRepository
    {
        public AddressRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public Guid? GetCity(string name)
        {
            var city = DbContext.Cities
                .FirstOrDefault(x => x.Ru.ToLower() == name.ToLower());

            if (city == null)
            {
                return null;
            }

            return city.ID;
        }

        public Guid? AddCity(string cityName, string countryName)
        {
            var shortCountry = countryName.ToUpper();
            var country = DbContext.Countries.FirstOrDefault(x => x.Shortname == shortCountry);
            Guid? cityId = null;
            if (country != null)
            {
                var city = new CityData
                {
                    CountryID = country.ID,
                    Name = cityName,
                    Ru = cityName,
                };
                var cityData = DbContext.Cities.Add(city);
                DbContext.SaveChanges();

                cityId = cityData.Entity.ID;
            }

            return cityId;
        }
    }
}
