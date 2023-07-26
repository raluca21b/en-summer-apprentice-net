using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Task<Order> GetById(int id);
        void Update(Order order);
        void Delete(Order order);
    }
}
