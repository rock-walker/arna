using AP.EntityModel.AutoDomain;
using AP.ViewModel.Workshop;
using AutoMapper;

namespace AP.Server.ViewMapping
{
    public class WorkshopProfile : Profile
    {
        public WorkshopProfile()
        {
            CreateMap<WorkshopViewModel, WorkshopData>().ReverseMap();
            CreateMap<WorkshopShortViewModel, WorkshopData>().ReverseMap();
        }
    }
}
