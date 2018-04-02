using AP.Core.Model.User;

namespace AP.Shared.Security.Contracts
{
    public interface IIdentityService
    {
        void PersistRefreshToken(RefreshToken refreshToken, ApplicationUser user);
        JwtIdentity GetRefreshToken(string refreshToken);
    }
}
