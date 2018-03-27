using AP.Core.Model;
using AP.Core.Model.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AP.Shared.Security.Contracts
{
    public interface IIdentityProvider
    {
        Task<Tuple<ApplicationUser, IdentityStatus, IList<string>>> Login(LoginInfo info);
    }
}
