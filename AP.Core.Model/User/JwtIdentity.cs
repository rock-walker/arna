using System.Collections.Generic;

namespace AP.Core.Model.User
{
    public class JwtIdentity
    {
        public ApplicationUser User { get; set; }
        public IdentityStatus LoggedInStatus { get; set; }
        public IList<string> Roles { get; set; }
        public RefreshToken RefreshToken { get; set; }
    }
}