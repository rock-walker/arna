using AP.EntityModel.AttendeeDomain;
using AP.Repository.Attendee.Contracts;
using AP.ViewModel.Attendee;
using AutoMapper;
using EntityFramework.DbContextScope.Interfaces;
using System;

namespace AP.Business.Attendee
{
    public class AttendeeAccountService : IAttendeeAccountService
    {
        private readonly IAttendeeAccountRepository attendeeRepository;
        private readonly IDbContextScopeFactory contextScope;

        public AttendeeAccountService(IAttendeeAccountRepository attendeeRepository, IDbContextScopeFactory contextScope)
        {
            this.attendeeRepository = attendeeRepository;
            this.contextScope = contextScope;
        }

        public Guid Register(AttendeeAccountViewModel viewModel, Guid userId)
        {
            var data = Mapper.Map<AttendeeData>(viewModel);
            data.UserID = userId;
            Guid attendeeId;

            using (var scope = contextScope.Create())
            {
                attendeeId = attendeeRepository.Add(data);
                scope.SaveChanges();
            }

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
