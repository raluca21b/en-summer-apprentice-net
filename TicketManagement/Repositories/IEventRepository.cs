using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAll();
        Task<Event> GetById(int id);
        void Update(Event @event);
        void Delete(Event @event);

    }
}
