using Microsoft.EntityFrameworkCore;
using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly PracticaContext _dbContext;

        public OrderRepository()
        {
            _dbContext = new PracticaContext();            
        }
        public IEnumerable<Order> GetAll()
        {
            var orders = _dbContext.Orders.Include(o => o.TicketCategory);
            return orders;
        }

        public Order GetById(int id)
        {
            var order = _dbContext.Orders.Include(o => o.TicketCategory).Where(o => o.OrderId == id).FirstOrDefault();
            return order;
        }
    }
}
