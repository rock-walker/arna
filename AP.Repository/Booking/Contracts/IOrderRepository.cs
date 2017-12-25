using AP.EntityModel.Booking;
using System;
using System.Linq.Expressions;

namespace AP.Repository.Booking.Contracts
{
    public interface IOrderRepository
    {
        void Add(Order order);
        Order GetAnchor(Expression<Func<Order, bool>> lookup);
        Order Find(Guid id);
        void Remove(Order order);
    }
}
