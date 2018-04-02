using AP.Core.Model.User;
using AP.Shared.Security.Contracts;
using AP.Repository.Common.Contracts;
using EntityFramework.DbContextScope.Interfaces;
using AutoMapper;
using EL.EntityModel;

namespace AP.Shared.Security.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IIdentityRepository repository;
        private readonly IDbContextScopeFactory factory;

        public IdentityService(IIdentityRepository repository, IDbContextScopeFactory factory)
        {
            this.repository = repository;
            this.factory = factory;
        }

        public void PersistRefreshToken(RefreshToken refreshToken, ApplicationUser user)
        {
            var tokenData = Mapper.Map<UserRefreshTokenData>(refreshToken);
            //tokenData.User = user;
            tokenData.UserID = user.Id;

            repository.PersistRefreshToken(tokenData);
        }

        public JwtIdentity GetRefreshToken(string refreshToken)
        {
            JwtIdentity identity = new JwtIdentity();
            var tokenData = repository.GetRefreshToken(refreshToken);
            if (tokenData != null)
            {
                identity.RefreshToken = Mapper.Map<RefreshToken>(tokenData);
                identity.User = tokenData.User;
            }
            return identity;
        }
    }
}
