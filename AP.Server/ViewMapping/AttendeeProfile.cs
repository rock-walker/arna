using AP.EntityModel.AttendeeDomain;
using AP.ViewModel.Attendee;
using AutoMapper;

namespace AP.Server.ViewMapping
{
    public class AttendeeProfile : Profile
    {
        public AttendeeProfile()
        {
            CreateMap<AttendeeAccountViewModel, AttendeeData>().ReverseMap();
            CreateMap<AttendeeAutoViewModel, AttendeeAutoData>().ReverseMap();
        }
    }
}
