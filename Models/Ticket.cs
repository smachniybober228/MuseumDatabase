using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Ticket
{
    public int Id { get; set; }

    public string TicketNumber { get; set; } = null!;

    public DateTime SaleDateTime { get; set; }

    public double SalePrice { get; set; }

    public DateTime VisitDate { get; set; }

    public int ExhibitionFk { get; set; }

    public int TicketStatusFk { get; set; }

    public virtual Exhibition ExhibitionFkNavigation { get; set; } = null!;

    public virtual TicketStatus TicketStatusFkNavigation { get; set; } = null!;
}
