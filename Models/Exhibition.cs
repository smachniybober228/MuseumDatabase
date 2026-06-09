using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Exhibition
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int HallFk { get; set; }

    public int ResponsibleCuratorFk { get; set; }

    public int ExhibitionStatusFk { get; set; }

    public virtual ICollection<ExhibitOnExhibition> ExhibitOnExhibitions { get; set; } = new List<ExhibitOnExhibition>();

    public virtual ExhibitionStatus ExhibitionStatusFkNavigation { get; set; } = null!;

    public virtual Hall HallFkNavigation { get; set; } = null!;

    public virtual Person ResponsibleCuratorFkNavigation { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
