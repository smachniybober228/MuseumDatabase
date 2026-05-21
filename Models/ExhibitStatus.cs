using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExhibitStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Exhibit> Exhibits { get; set; } = new List<Exhibit>();
}
