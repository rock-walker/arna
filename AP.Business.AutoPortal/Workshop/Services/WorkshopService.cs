using AP.ViewModel.Workshop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AP.Business.AutoDomain.Workshop.Contracts;
using AP.Repository.Workshop.Contracts;

namespace AP.Business.AutoDomain.Workshop.Services
{
    public class WorkshopService : IWorkshopService
    {
        private readonly IWorkshopRepository _repo;

        public WorkshopService(IWorkshopRepository repository)
        {
            _repo = repository;
        }
        public async Task<IEnumerable<WorkshopViewModel>> GetAll()
        {
            return await Task.Run(() => _repo.GetAll());
        }

        public async Task<IEnumerable<WorkshopViewModel>> GetById(IEnumerable<Guid> ids)
        {
            return await Task.Run(() => _repo.GetById(ids));
        }

        public Task<IEnumerable<WorkshopViewModel>> GetByCity(string city)
        {
            throw new NotImplementedException();
        }
    }
}
