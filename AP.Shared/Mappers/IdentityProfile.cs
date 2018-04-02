using AP.Core.Model.User;
using AutoMapper;
using EL.EntityModel;

namespace AP.Shared.Mappers
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile()
        {
            CreateMap<RefreshToken, UserRefreshTokenData>().ReverseMap();
        }
    }
}
