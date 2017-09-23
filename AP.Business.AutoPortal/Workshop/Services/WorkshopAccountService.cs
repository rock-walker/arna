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

namespace AP.Business.AutoPortal.Workshop.Services
{
    public class WorkshopAccountService : IWorkshopAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWorkshopAccountRepository _workshopAccountRepository;
        private readonly IDbContextScopeFactory _dbContextScope;

        public WorkshopAccountService(UserManager<ApplicationUser> userManager, 
            IWorkshopAccountRepository accountRepo,
            IDbContextScopeFactory scopeFactory)
        {
            _userManager = userManager;
            _workshopAccountRepository = accountRepo;
            _dbContextScope = scopeFactory;
        }

        public async Task Add(WorkshopAccountViewModel workshopViewModel)
        {
            using (var dbContext = _dbContextScope.Create())
            {
                var workshopData = Mapper.Map<WorkshopData>(workshopViewModel);
                await _workshopAccountRepository.Add(workshopData);
                dbContext.SaveChanges();
            }
        }

        public async Task Update(WorkshopAccountViewModel workshopViewModel)
        {
            throw new NotImplementedException();
        }

        public async Task<WorkshopShortViewModel> FindByName(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetAccountPhone(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user.PhoneNumber;
        }
    }
}
