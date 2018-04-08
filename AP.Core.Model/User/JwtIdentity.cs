using System.Collections.Generic;
using System.Security.Claims;

namespace AP.Core.Model.User
{
    public class JwtIdentity
    {
        public ApplicationUser User { get; set; }
        public IdentityStatus LoggedInStatus { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}