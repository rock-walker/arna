using AP.ViewModel.Workshop;
using AP.EntityModel.Mappers;
using AP.Repository.Context;
using AP.Repository.Workshop.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AP.Repository.Workshop.Services
{
    public class WorkshopRepository : IWorkshopRepository
    {
        private readonly WorkshopContext _ctx;

        public WorkshopRepository(WorkshopContext context)
        {
            _ctx = context;
        }

        public async Task<IEnumerable<WorkshopViewModel>> GetAll()
        {
            return await Task.Run(() =>
            {
                var workshops = _ctx.Workshops;
                return workshops
                        .Include(x => x.WorkshopCategories)
                            .ThenInclude(x => x.Category)
                        .Include(x => x.Contact)
                        .Include(x => x.AutoBrand)
                        .Include(x => x.Address)
                            .ThenInclude(x => x.City)
                        .Include(x => x.Location)
                        .Include(x => x.Logo)
                        .AsNoTracking()
                        .Select(x => x.MapTo());
            });
        }
    }
}
