﻿using AP.Business.Registration.Commands;
using AP.EntityModel.Booking;
using AP.ViewModel.Workshop;
using AutoMapper;

namespace AP.Server.ViewMapping
{
    public class WorkshopOrderProfile : Profile
    {
        public WorkshopOrderProfile()
        {
            CreateMap<OrderAnchor, AssignAnchor>();
            CreateMap<AnchorTypeViewModel, AnchorType>().ReverseMap();
        }
    }
}
