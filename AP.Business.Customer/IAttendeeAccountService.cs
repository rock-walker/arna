using AP.Core.Model.User;
using AP.ViewModel.Attendee;
using System;
using System.Threading.Tasks;

namespace AP.Business.Attendee
{
    public interface IAttendeeAccountService
    {
        Task<Guid> Register(AttendeeAccountViewModel viewModel, ApplicationUser user);
        void Update();
        void Remove();
        void Test();
    }
}
