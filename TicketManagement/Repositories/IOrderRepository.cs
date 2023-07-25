using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetAll();
        Order GetById(int id);
    }
}
