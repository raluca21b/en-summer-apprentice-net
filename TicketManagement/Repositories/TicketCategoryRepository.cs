﻿using Microsoft.EntityFrameworkCore;
using TicketManagement.Exceptions;
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
        public async Task<TicketCategory> GetTicketCategoryByEventIdAndDescription(int eventId, string description)
        {
            var ticketCategory = await _dbContext.TicketCategories.Include(tc => tc.Event)
                                                            .FirstOrDefaultAsync(tc => tc.EventId == eventId &&
                                                                            tc.Description == description);
            if (ticketCategory == null)
            {
                throw new EntityNotFoundException("TicketCategory not found with eventId " + eventId.ToString()
                                                                                           + " and description "
                                                                                           + description);
            }
            return ticketCategory;
        }
    }
}
