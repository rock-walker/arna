using AP.Repository.Booking.Contracts;
using AP.EntityModel.Booking;
using AP.Core.Database;
using AP.Repository.Context;
using EntityFramework.DbContextScope.Interfaces;
using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AP.Repository.Booking.Services
{
    public class OrderRepository : AmbientContext<WorkshopContext>, IOrderRepository
    {
        private readonly Guid creationId;

        public OrderRepository(IAmbientDbContextLocator locator) : base(locator)
        {
            creationId = Guid.NewGuid();
        }
        public void Add(Order order)
        {
            DbContext.Add(order);
        }

        public Order GetAnchor(Expression<Func<Order, bool>> lookup)
        {
            return DbContext.Orders.Include(x => x.Anchors).FirstOrDefault(lookup);
        }

        public Order Find(Guid id)
        {
            return DbContext.Orders.FirstOrDefault(x => x.Id == id);
        }

        public void Remove (Order order)
        {
            DbContext.Orders.Remove(order);
        }
    }
}
