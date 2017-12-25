using AP.EntityModel.AttendeeDomain;
using System;

namespace AP.Repository.Attendee.Contracts
{
    public interface IAttendeeAccountRepository
    {
        Guid Add(AttendeeData model);
        void Update();
        void Remove();
        AttendeeData Find(Guid id);
    }
}
