using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExhibitionStatus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();
}
