using AP.Repository.Common.Contracts;
using System;
using AP.Repository.Context;
using EntityFramework.DbContextScope.Interfaces;
using System.Linq;
using AP.Core.Database;

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
    }
}
