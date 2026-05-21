using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Hall
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string HallNumber { get; set; } = null!;

    public int FloorFk { get; set; }

    public virtual ICollection<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();

    public virtual FloorEntity FloorFkNavigation { get; set; } = null!;
}
