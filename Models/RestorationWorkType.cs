using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class RestorationWorkType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<RequiredWorkType> RequiredWorkTypes { get; set; } = new List<RequiredWorkType>();

    public virtual ICollection<WorkLogEntry> WorkLogEntries { get; set; } = new List<WorkLogEntry>();
}
