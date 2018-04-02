using AP.Business.AutoPortal.Workshop.Contracts;
using System;
using System.Collections.Generic;
using AP.ViewModel.Workshop;
using System.Threading.Tasks;
using AP.Repository.Workshop.Contracts;
using AutoMapper;
using EntityFramework.DbContextScope.Interfaces;

namespace AP.Business.AutoPortal.Workshop.Services
{
    public class WorkshopFilterService : IWorkshopFilterService
    {
        private readonly IWorkshopFilterRepository _filterRepository;
        private readonly IDbContextScopeFactory scopeFactory;

        public WorkshopFilterService(IWorkshopFilterRepository filterRepository, IDbContextScopeFactory factory)
        {
            _filterRepository = filterRepository;
            this.scopeFactory = factory;
        }

        public WorkshopViewModel FindById(Guid id)
        {
            try
            {
                using (var scope = scopeFactory.CreateReadOnly())
                {
                    var workshop = _filterRepository.FindById(id);
                    return Mapper.Map<WorkshopViewModel>(workshop);
                }
            }
            catch (Exception)
            {
                throw new KeyNotFoundException(string.Format("Workshop with Id {0} doesn't exist in DB", id));
            }
        }

        public WorkshopViewModel FindBySlug(string slug)
        {
            using (var scope = scopeFactory.CreateReadOnly())
            {
                var workshop = _filterRepository.FindBySlug(slug);
                return Mapper.Map<WorkshopViewModel>(workshop);
            }
        }

        public async Task<WorkshopShortViewModel> FindByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
