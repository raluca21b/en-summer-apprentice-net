using Microsoft.EntityFrameworkCore;
using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly PracticaContext _dbContext;

        public EventRepository()
        {
            _dbContext = new PracticaContext();
        }
        public int Add(Event @event)
        {
            throw new NotImplementedException();
        }

        public void Delete(Event @event)
        {
            _dbContext.Remove(@event);
            _dbContext.SaveChanges();
        }

        public async Task<IEnumerable<Event>> GetAll()
        {
            var events = await _dbContext.Events.Include(e => e.EventType)
                                          .Include(e => e.Venue).ToListAsync();
            return events;
        }

        public async Task<Event> GetById(int id)
        {
            var @event = await _dbContext.Events.Include(e => e.EventType)
                                                .Include(e => e.Venue)
                                                .Where(e => e.EventId == id)
                                                .FirstOrDefaultAsync();
            return @event;
        }

        public void Update(Event @event)
        {
            _dbContext.Entry(@event).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
