using AP.Business.AutoPortal.Workshop.Contracts;
using System;
using System.Collections.Generic;
using AP.ViewModel.Workshop;
using System.Threading.Tasks;
using AP.Repository.Workshop.Contracts;
using AutoMapper;

namespace AP.Business.AutoPortal.Workshop.Services
{
    public class WorkshopFilterService : IWorkshopFilterService
    {
        private readonly IWorkshopFilterRepository _filterRepository;

        public WorkshopFilterService(IWorkshopFilterRepository filterRepository)
        {
            _filterRepository = filterRepository;
        }

        public async Task<WorkshopViewModel> FindById(Guid id)
        {
            var workshop = await _filterRepository.FindById(id);
            if (workshop == null)
            {
                throw new KeyNotFoundException(string.Format("Workshop with that Id {id} doesn't exist in DB", id));
            }

            return Mapper.Map<WorkshopViewModel>(workshop);
        }

        public async Task<WorkshopShortViewModel> FindByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
