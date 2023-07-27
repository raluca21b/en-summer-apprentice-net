using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAll();
        Task<Event> GetById(int id);
        Task Update(Event @event);
        Task Delete(Event @event);

    }
}
