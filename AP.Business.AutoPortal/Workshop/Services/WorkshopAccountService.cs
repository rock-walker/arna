using AP.Business.AutoPortal.Workshop.Contracts;
using System;
using AP.ViewModel.Workshop;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AP.Core.Model.User;
using AP.Repository.Workshop.Contracts;
using AutoMapper;
using AP.EntityModel.AutoDomain;
using EntityFramework.DbContextScope.Interfaces;
using AP.Business.Domain.Common;

namespace AP.Business.AutoPortal.Workshop.Services
{
    public class WorkshopAccountService : IWorkshopAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAddressService _addressService;
        private readonly IWorkshopFilterService _filterService;
        private readonly IWorkshopAccountRepository _workshopAccountRepository;
        private readonly IDbContextScopeFactory _dbContextScope;

        public WorkshopAccountService(UserManager<ApplicationUser> userManager, 
            IWorkshopAccountRepository accountRepo,
            IDbContextScopeFactory scopeFactory,
            IAddressService addressService,
            IWorkshopFilterService filterService)
        {
            _addressService = addressService;
            _filterService = filterService;
            _userManager = userManager;
            _workshopAccountRepository = accountRepo;
            _dbContextScope = scopeFactory;
        }

        public async Task<Guid> Add(WorkshopAccountViewModel workshopViewModel)
        {
            //ensure, that workshop doesn't exist at DB
            var workshopData = Mapper.Map<WorkshopAccountViewModel, WorkshopData>(workshopViewModel);

            var workshopId = Guid.Empty;
            using (var dbContext = _dbContextScope.Create())
            {
                var cityId = _addressService.GetCityByName(workshopData.Address.City.Ru);
                if (cityId == null)
                {
                    throw new ArgumentException(string.Format("the city {0} hasn't been added to database", workshopData.Address.City.Ru));
                }

                workshopData.Address.CityID = cityId;

                workshopId = await _workshopAccountRepository.Add(workshopData);
                await dbContext.SaveChangesAsync();
            }

            return workshopId;
        }

        public async Task Update(WorkshopAccountViewModel workshopViewModel)
        {
            var workshopData = Mapper.Map<WorkshopAccountViewModel, WorkshopData>(workshopViewModel);

            using (var dbContext = _dbContextScope.Create())
            {
                var workshopDb = await _filterService.FindById(workshopData.ID);
                if (workshopDb == null)
                {
                    throw new ArgumentException(string.Format("Provided ID {0} doesn't exist in DB", workshopData.ID));
                }

                if (workshopDb.Address != null && workshopDb.Address.City.Ru != workshopData.Address.City.Ru)
                {
                    //verify if new city exists in DB
                    var cityId = _addressService.GetCityByName(workshopData.Address.City.Ru);
                    if (cityId == null)
                    {
                        throw new ArgumentException(string.Format("the city {0} hasn't been added to database", workshopData.Address.City.Ru));
                    }

                    workshopData.Address.CityID = cityId;
                }

                var categories = workshopData.WorkshopCategories;
                if (categories != null)
                {
                    foreach(var c in categories)
                    {
                        c.WorkshopID = workshopData.ID;
                    }
                }

                var autobrands = workshopData.WorkshopAutobrands;
                if (autobrands != null)
                {
                    foreach(var w in autobrands)
                    {
                        w.WorkshopID = workshopData.ID;
                    }
                }

                _workshopAccountRepository.Update(workshopData);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<string> GetAccountPhone(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user.PhoneNumber;
        }
    }
}
