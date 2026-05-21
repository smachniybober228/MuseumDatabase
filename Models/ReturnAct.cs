using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ReturnAct
{
    public int Id { get; set; }

    public DateOnly ReturnDate { get; set; }

    public int RestorationOrderFk { get; set; }

    public virtual RestorationOrderEntity RestorationOrderFkNavigation { get; set; } = null!;
}
