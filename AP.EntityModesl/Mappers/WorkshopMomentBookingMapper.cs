using AP.Business.Model.Workshop;

namespace AP.EntityModel.Mappers
{
    public static class WorkshopMomentBookingMapper
    {
        public static BookingMomentModel MapTo(this AutoDomain.WorkshopCategoryData data)
        {
            return new BookingMomentModel
            {
                Category = data.Category.MapTo(),
                WorkshopId = data.WorkshopID
            };
        }
    }
}
