using AP.EntityModel.AutoDomain;
using AP.EntityModel.Common;
using AP.ViewModel.Common;
using AP.ViewModel.Workshop;
using AutoMapper;

namespace AP.Server.Application
{
    public class WorkshopProfile : Profile
    {
        public WorkshopProfile()
        {
            CreateMap<WorkshopViewModel, WorkshopData>().ReverseMap();
        }
    }
}
