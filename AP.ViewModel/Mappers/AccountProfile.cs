using AP.Core.Model.User;
using AP.ViewModel.Account;
using AutoMapper;

namespace AP.ViewModel.Mappers
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<LoginViewModel, LoginInfo>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.Email));
        }
    }
}
