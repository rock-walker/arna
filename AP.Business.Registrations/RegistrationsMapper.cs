using AP.Business.Model.Registration.Events;
using AP.Business.Registration;
using AP.EntityModel.ReadModel;
using AutoMapper;

namespace AP.Business.Registrations
{
    public class RegistrationsMapper : Profile
    {
        public RegistrationsMapper()
        {
            CreateMap<OrderPaymentConfirmed, OrderConfirmed>();
            CreateMap<AnchorAssigned, AnchorAssignments>();
            CreateMap<AnchorUnassigned, AnchorAssignments>();
            CreateMap<AnchorAssignmentUpdated, AnchorAssignments>();
            CreateMap<AnchorAssigned, OrderAnchor>();
            CreateMap<AnchorAssignmentUpdated, OrderAnchor>();

        }
    }
}
