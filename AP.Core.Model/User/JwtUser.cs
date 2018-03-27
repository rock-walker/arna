using System.Collections.Generic;

namespace AP.Core.Model.User
{
    public class JwtUser
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
