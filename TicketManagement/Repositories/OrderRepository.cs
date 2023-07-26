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

        public void Delete(Order order)
        {
            _dbContext.Remove(order);   
            _dbContext.SaveChanges();
        }

        public IEnumerable<Order> GetAll()
        {
            var orders = _dbContext.Orders.Include(o => o.TicketCategory)
                                          .ThenInclude(tc => tc.Event);
            return orders;
        }

        public async Task<Order> GetById(int id)
        {
            var order = await _dbContext.Orders.Include(o => o.TicketCategory)
                                         .ThenInclude(tc => tc.Event)
                                         .FirstOrDefaultAsync(o => o.OrderId == id);

            return order;
        }

        public void Update(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
