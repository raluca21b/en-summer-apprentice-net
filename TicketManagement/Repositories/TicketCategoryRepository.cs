﻿using Microsoft.EntityFrameworkCore;
using TicketManagement.Models;

namespace TicketManagement.Repositories
{
    public class TicketCategoryRepository : ITicketCategoryRepository
    {
        private readonly PracticaContext _dbContext;
        public TicketCategoryRepository()
        {
            _dbContext = new PracticaContext();   
        }
        public TicketCategory GetTicketCategoryByEventIdAndDescription(int eventId, string description)
        {
            var ticketCategory = _dbContext.TicketCategories.Include(tc => tc.Event)
                                                            .FirstOrDefault(tc => tc.EventId == eventId &&
                                                                            tc.Description == description);
            Console.WriteLine(ticketCategory);
            return ticketCategory;
        }
    }
}
