using AP.Core.Model;
using AP.Core.Model.User;
using System.Threading.Tasks;

namespace AP.Shared.Security.Contracts
{
    public interface IIdentityProvider
    {
        Task<IdentityStatus> SignIn(LoginInfo info);
        Task<JwtIdentity> OauthSignIn(LoginInfo info);
        Task<JwtIdentity> GetRefreshToken(string verifyRefreshToken);
        RefreshToken GenerateRefreshToken(JwtIdentity info);
        void RevokeAccessToken(string token);
        Task<JwtIdentity> ProvideOauthWorkflow(LoginInfo info);
    }
}
