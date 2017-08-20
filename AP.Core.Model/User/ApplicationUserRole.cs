using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AP.Core.Model.User
{
    public class ApplicationUserLogin: IdentityUserLogin<Guid> { }
    public class ApplicationRoleClaim: IdentityRoleClaim<Guid> { }
    public class ApplicationUserToken: IdentityUserToken<Guid> { }
}
