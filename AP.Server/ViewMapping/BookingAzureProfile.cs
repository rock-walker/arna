using AP.Infrastructure.Azure.EventSourcing;
using AutoMapper;

namespace AP.Server.ViewMapping
{
    public class BookingAzureProfile : Profile
    {
        public BookingAzureProfile()
        {
            CreateMap<EventTableServiceEntity, EventData>().ReverseMap();
        }
    }
}
