using Microsoft.Extensions.DependencyInjection;

namespace AP.Repository.Application.Contracts
{
    public interface IContainerRegister
    {
        void RegisterDependency(IServiceCollection services);
    }
}
