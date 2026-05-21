using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExhibitMaterial
{
    public int Id { get; set; }

    public int ExhibitFk { get; set; }

    public int MaterialFk { get; set; }

    public virtual Exhibit ExhibitFkNavigation { get; set; } = null!;

    public virtual Material MaterialFkNavigation { get; set; } = null!;
}
