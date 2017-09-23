using AP.EntityModel.AutoDomain;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Contracts
{
    public interface IWorkshopAccountRepository
    {
        Task Add(WorkshopData account);
        Task Update(WorkshopData account);
    }
}
