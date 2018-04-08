using AP.Core.Model.User;
using System.Threading.Tasks;

namespace AP.Shared.Security.Contracts
{
    public interface IAccountService
    {
        Task AddRole(ApplicationUser user, Roles role);
        Task AddClaim(ApplicationUser user, ApplicationClaims appClaim);
        Task<ApplicationUser> FindById(string userId);
        Task<ApplicationUser> FindByEmail(string email);
        Task<JwtResponse> CompleteUserVerification(ApplicationUser user, string phoneNumber, string code);
        Task<JwtResponse> RefreshJwt(ApplicationUser user);
    }
}
