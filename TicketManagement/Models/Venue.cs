using System;
using System.Collections.Generic;

namespace TicketManagement.Models;

public partial class Venue
{
    public int VenueId { get; set; }

    public int? Capacity { get; set; }

    public string? Location { get; set; }

    public string? Type { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
