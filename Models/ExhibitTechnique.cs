using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExhibitTechnique
{
    public int Id { get; set; }

    public int ExhibitFk { get; set; }

    public int TechniqueFk { get; set; }

    public virtual Exhibit ExhibitFkNavigation { get; set; } = null!;

    public virtual Technique TechniqueFkNavigation { get; set; } = null!;
}
