using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class FloorEntity
{
    public int Id { get; set; }

    public int FloorNumber { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Hall> Halls { get; set; } = new List<Hall>();
}
