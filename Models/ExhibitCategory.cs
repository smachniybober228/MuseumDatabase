using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExhibitCategory
{
    public int Id { get; set; }

    public int ExhibitFk { get; set; }

    public int CategoryFk { get; set; }

    public virtual Category CategoryFkNavigation { get; set; } = null!;

    public virtual Exhibit ExhibitFkNavigation { get; set; } = null!;
}
