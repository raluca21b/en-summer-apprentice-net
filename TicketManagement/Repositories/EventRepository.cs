using Microsoft.EntityFrameworkCore;
using TicketManagement.Exceptions;
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

        public async Task Delete(Event @event)
        {
            _dbContext.Remove(@event);
            await _dbContext.SaveChangesAsync();
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

            if (@event == null)
            {
                throw new EntityNotFoundException(id, nameof(Event));
            }

            return @event;
        }

        public async Task Update(Event @event)
        {
            _dbContext.Entry(@event).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}
