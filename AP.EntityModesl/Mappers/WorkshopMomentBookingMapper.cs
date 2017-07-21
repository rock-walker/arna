using AP.ViewModel.Workshop;

namespace AP.EntityModel.Mappers
{
    public static class WorkshopMomentBookingMapper
    {
        public static BookingMomentViewModel MapTo(this AutoDomain.WorkshopCategory data)
        {
            return new BookingMomentViewModel
            {
                Category = data.Category.MapTo(),
                MomentStatus = data.MomentBookingState,
                WorkshopId = data.WorkshopID
            };
        }
    }
}
