using AP.Core.Database;
using AP.EntityModel.AttendeeDomain;
using AP.Repository.Context;
using AP.Repository.Attendee.Contracts;
using EntityFramework.DbContextScope.Interfaces;
using System;

namespace AP.Repository.Attendee.Services
{
    public class AttendeeAccountRepository : AmbientContext<AttendeeContext>, IAttendeeAccountRepository
    {
        public AttendeeAccountRepository(IAmbientDbContextLocator locator) : base(locator)
        {
        }

        public Guid Add(AttendeeData model)
        {
            DbContext.Add(model);
            return model.ID;
        }

        public void Remove()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public AttendeeData Find(Guid id)
        {
            return DbContext.Find<AttendeeData>(id);
        }
    }
}
