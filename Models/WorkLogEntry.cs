using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class WorkLogEntry
{
    public int Id { get; set; }

    public DateTime ExecutionDate { get; set; }

    public int RestorationOrderFk { get; set; }

    public int WorkTypeFk { get; set; }

    public virtual RestorationOrderEntity RestorationOrderFkNavigation { get; set; } = null!;

    public virtual RestorationWorkType WorkTypeFkNavigation { get; set; } = null!;
}
