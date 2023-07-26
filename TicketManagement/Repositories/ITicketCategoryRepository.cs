using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface ITicketCategoryRepository
    {
        TicketCategory GetTicketCategoryByEventIdAndDescription(int eventId, string description);
    }
}
