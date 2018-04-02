// ==============================================================================================================
namespace AP.Business.Registration.ReadModel
{
    using AP.EntityModel.ReadModel;
    using System;
    using System.Collections.Generic;

    public interface IWorkshopDao
    {
        WorkshopDetails GetWorkshopDetails(string workshopCode);
        WorkshopAlias GetWorkshopAlias(string workshopCode);

        IList<WorkshopAlias> GetPublishedWorkshops();
        IList<AnchorType> GetPublishedAnchorTypes(Guid workshopId);
        IList<AnchorTypeName> GetAnchorTypeNames(IEnumerable<Guid> seatTypes);
    }
}