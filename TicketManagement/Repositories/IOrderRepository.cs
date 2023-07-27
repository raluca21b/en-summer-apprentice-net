using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAll();
        Task<Order> GetById(int id);
        Task Update(Order order);
        Task Delete(Order order);
    }
}
