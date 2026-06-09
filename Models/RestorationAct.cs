using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class RestorationAct
{
    public int Id { get; set; }

    public DateTime CompletionDate { get; set; }

    public string FinalReport { get; set; } = null!;

    public double TotalCost { get; set; }

    public int RestorationOrderFk { get; set; }

    public virtual RestorationOrderEntity RestorationOrderFkNavigation { get; set; } = null!;
}
