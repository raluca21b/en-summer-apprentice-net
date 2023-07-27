using Microsoft.EntityFrameworkCore;
using TicketManagement.Exceptions;
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

        public async Task Delete(Order order)
        {
            _dbContext.Remove(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            var orders = await _dbContext.Orders.Include(o => o.TicketCategory)
                                          .ThenInclude(tc => tc.Event).ToListAsync();
            return orders;
        }

        public async Task<Order> GetById(int id)
        {
            var order = await _dbContext.Orders.Include(o => o.TicketCategory)
                                         .ThenInclude(tc => tc.Event)
                                         .FirstOrDefaultAsync(o => o.OrderId == id);


            if (order == null)
            {
                throw new EntityNotFoundException(id, nameof(Order));
            }

            return order;
        }

        public async Task Update(Order order)
        {
            _dbContext.Entry(order).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
