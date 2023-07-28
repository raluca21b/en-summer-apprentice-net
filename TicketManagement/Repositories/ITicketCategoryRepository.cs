using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public interface ITicketCategoryRepository
    {
        Task<TicketCategory> GetTicketCategoryByEventIdAndDescription(int eventId, string description);
    }
}
