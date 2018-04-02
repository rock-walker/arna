using AP.Business.Model.Registration.Events;
using AP.EntityModel.ReadModel;
using AutoMapper;

namespace AP.Business.Registrations
{
    public class RegistrationsMapper : Profile
    {
        public RegistrationsMapper()
        {
            CreateMap<AnchorAssigned, Registration.AnchorAssignments.AnchorAssignment>();
            CreateMap<AnchorUnassigned, Registration.AnchorAssignments.AnchorAssignment>();
            CreateMap<AnchorAssignmentUpdated, Registration.AnchorAssignments.AnchorAssignment>();
            CreateMap<AnchorAssigned, OrderAnchor>();
            CreateMap<AnchorAssignmentUpdated, OrderAnchor>();
        }
    }
}
