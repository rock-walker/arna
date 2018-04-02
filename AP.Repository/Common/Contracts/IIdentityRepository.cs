using AP.Core.Model.User;
using EL.EntityModel;

namespace AP.Repository.Common.Contracts
{
    public interface IIdentityRepository
    {
        void PersistRefreshToken(UserRefreshTokenData refreshToken);
        UserRefreshTokenData GetRefreshToken(string refreshToken);
    }
}
