using AP.EntityModel.Common;
using System;
using System.Threading.Tasks;

namespace AP.Repository.Common.Contracts
{
    public interface IAddressRepository
    {
        Guid? GetCity(string name);
    }
}
