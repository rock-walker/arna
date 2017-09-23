using AP.EntityModel.AutoDomain;
using AP.ViewModel.Workshop;
using AutoMapper;

namespace AP.Server.Application
{
    public class AutomapperConfig
    {
        public static void RegisterModels()
        {
            Mapper.Initialize(cfg => cfg.CreateMap<WorkshopAccountViewModel, WorkshopData>());
        }
    }
}
