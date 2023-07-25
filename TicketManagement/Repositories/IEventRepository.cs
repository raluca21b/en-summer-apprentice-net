using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAll();
        Event GetById(int id);
        int Add(Event @event);
        void Update(Event @event);
        int Delete(int id);

    }
}
