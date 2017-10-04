using AP.EntityModel.AutoDomain;
using AP.EntityModel.Common;
using AP.ViewModel.Common;
using AP.ViewModel.Workshop;
using AP.Core.GeoLocation;
using AutoMapper;
using AP.Business.Model.Common;

namespace AP.Server.Application
{
    public class WorkshopAccountProfile : Profile
    {
        public WorkshopAccountProfile()
        {
            CreateMap<WorkshopAccountViewModel, WorkshopData>().ReverseMap();
            CreateMap<ContactViewModel, ContactData>().ReverseMap();
            CreateMap<LocationViewModel, GeoMarker>()
                .ForMember(dest => dest.Lat, opt => opt.MapFrom(src => GeoLocation.ConvertDegreesToRadians(src.Lat)))
                .ForMember(dest => dest.Lng, opt => opt.MapFrom(src => GeoLocation.ConvertDegreesToRadians(src.Lng)))
                .ReverseMap();

            CreateMap<AddressViewModel, AddressData>().ReverseMap();
            CreateMap<CityViewModel, CityData>().ReverseMap();
            CreateMap<CountryViewModel, CountryData>().ReverseMap();
            CreateMap<DayTimetableModel, WorkshopDayTimetable>().ReverseMap();
            CreateMap<WorkshopCategoryViewModel, WorkshopCategoryData>().ReverseMap();
            CreateMap<AutobrandViewModel, WorkshopAutoBrand>()
                .ForMember(src => src.AutoBrandID, opt => opt.MapFrom(src => src.ID))
                .ForMember(src => src.ID, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<AutobrandViewModel, AutoBrandData>().ReverseMap();
        }
    }
}
