using AP.Core.Model.User;
using AP.EntityModel.AttendeeDomain;
using AP.Repository.Attendee.Contracts;
using AP.Shared.Security.Contracts;
using AP.ViewModel.Attendee;
using AutoMapper;
using EntityFramework.DbContextScope.Interfaces;
using System;
using System.Threading.Tasks;

namespace AP.Business.Attendee
{
    public class AttendeeAccountService : IAttendeeAccountService
    {
        private readonly IAttendeeAccountRepository attendeeRepository;
        private readonly IDbContextScopeFactory contextScope;
        private readonly IAccountService accountService;

        public AttendeeAccountService(IAttendeeAccountRepository attendeeRepository, 
            IDbContextScopeFactory contextScope,
            IAccountService accountService)
        {
            this.attendeeRepository = attendeeRepository;
            this.contextScope = contextScope;
            this.accountService = accountService;
        }

        public async Task<Guid> Register(AttendeeAccountViewModel viewModel, ApplicationUser user)
        {
            var data = Mapper.Map<AttendeeData>(viewModel);
            data.UserID = user.Id;
            Guid attendeeId;

            using (var scope = contextScope.Create())
            {
                attendeeId = attendeeRepository.Add(data);
                scope.SaveChanges();
            }

            await accountService.AddRole(user, Roles.Client);
            return attendeeId;
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public void Test()
        {
            var id = Guid.Parse("CF262D98-3190-4513-269F-08D544B40A4B");
            using (var scope = contextScope.Create())
            {
                var attendeeId = attendeeRepository.Find(id);
            }

            using (var scope = contextScope.Create())
            {
                var attendeeId = attendeeRepository.Find(id);
            }
        }
    }
}
