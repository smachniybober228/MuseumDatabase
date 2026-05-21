using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class TicketStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
