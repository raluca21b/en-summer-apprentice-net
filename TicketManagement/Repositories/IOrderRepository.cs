using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAll();
        Task<Order> GetById(int id);
        void Update(Order order);
        void Delete(Order order);
    }
}
