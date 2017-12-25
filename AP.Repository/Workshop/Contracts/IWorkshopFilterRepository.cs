using AP.EntityModel.AutoDomain;
using System;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopFilterRepository
    {
        WorkshopData FindById(Guid id);
        Task<WorkshopData> FindByName(string name);
    }
}
