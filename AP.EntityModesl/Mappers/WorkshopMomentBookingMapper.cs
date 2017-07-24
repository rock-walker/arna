using AP.Business.Model.Workshop;

namespace AP.EntityModel.Mappers
{
    public static class WorkshopMomentBookingMapper
    {
        public static BookingMomentModel MapTo(this AutoDomain.WorkshopCategory data)
        {
            return new BookingMomentModel
            {
                Category = data.Category.MapTo(),
                MomentStatus = data.MomentBookingState,
                WorkshopId = data.WorkshopID
            };
        }
    }
}
