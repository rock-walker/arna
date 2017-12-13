using AutoMapper;

namespace AP.Server.Application
{
    public class AutomapperConfig
    {
        public static void RegisterModels()
        {
            Mapper.Initialize(cfg => cfg.AddProfiles(new[] { "AP.Server", "AP.Business.Registrations" }));
        }
    }
}
