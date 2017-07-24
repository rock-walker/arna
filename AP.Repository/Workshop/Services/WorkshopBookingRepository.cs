using AP.Business.Model.Workshop;
using AP.EntityModel.Mappers;
using AP.Repository.Context;
using AP.Repository.Workshop.Contracts;
using AP.ViewModel.Workshop;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopBookingRepository : IWorkshopBookingRepository
    {
        private readonly WorkshopContext _ctx;

        public WorkshopBookingRepository(WorkshopContext context)
        {
            _ctx = context;
        }

        public async Task<IEnumerable<BookingMomentModel>> GetMomentBookings(IEnumerable<Guid> workshops)
        {
            return await Task.Run(() =>
            {
                var dbWorkshops = _ctx.WorkshopCategory
                        .Include(x => x.Category)
                        .AsNoTracking();

                foreach (var workshop in workshops)
                {
                    dbWorkshops.Where(x => x.ID == workshop);
                }

                return dbWorkshops.Select(x => x.MapTo());
            });
        }
    }
}
