using AP.ViewModel.Attendee;
using System;

namespace AP.Business.Attendee
{
    public interface IAttendeeAccountService
    {
        Guid Register(AttendeeAccountViewModel viewModel, Guid userId);
        void Update();
        void Remove();
        void Test();
    }
}
