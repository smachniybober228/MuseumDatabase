using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class WriteOffAct
{
    public int Id { get; set; }

    public DateOnly WriteOffDate { get; set; }

    public string WriteOffReason { get; set; } = null!;

    public int ExhibitFk { get; set; }

    public virtual Exhibit ExhibitFkNavigation { get; set; } = null!;
}
