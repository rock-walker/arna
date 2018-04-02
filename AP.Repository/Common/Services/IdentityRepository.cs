using AP.Repository.Common.Contracts;
using AP.Repository.Context;
using System;
using EL.EntityModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AP.Repository.Common.Services
{
    public class IdentityRepository : IIdentityRepository
    {
        private readonly IdentityContext context;

        public IdentityRepository(IdentityContext context)
        {
            this.context = context;
        }

        public void PersistRefreshToken(UserRefreshTokenData refreshToken)
        {
            //TODO: should be there
            //using (var context = new IdentityContext())
            var oldToken = context.UserRefreshTokens
                .SingleOrDefault(x => x.UserID == refreshToken.UserID);

            if (oldToken != null)
            {
                context.UserRefreshTokens.Remove(oldToken);
            }

            context.UserRefreshTokens.Add(refreshToken);
            context.SaveChanges();
        }

        public UserRefreshTokenData GetRefreshToken(string refreshToken)
        {
            return context.UserRefreshTokens
                .Include(x => x.User)
                .AsNoTracking()
                .FirstOrDefault(x => string.Equals(x.Token, refreshToken, StringComparison.OrdinalIgnoreCase));
        }
    }
}
