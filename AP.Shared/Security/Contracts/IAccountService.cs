using AP.Core.Model.User;
using System.Threading.Tasks;

namespace AP.Shared.Security.Contracts
{
    public interface IAccountService
    {
        Task AddRole(ApplicationUser user, Roles role);
        Task<ApplicationUser> FindUserById(string userId);
        Task<JwtResponse> CompleteUserVerification(ApplicationUser user, string phoneNumber, string code);
        Task<JwtResponse> RefreshJwt(ApplicationUser user, string verifyRefreshToken);
    }
}
