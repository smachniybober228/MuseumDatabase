using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExhibitPhoto
{
    public int Id { get; set; }

    public string FileLink { get; set; } = null!;

    public DateOnly ShootingDate { get; set; }

    public int PhotoTypeFk { get; set; }

    public int ExhibitFk { get; set; }

    public virtual Exhibit ExhibitFkNavigation { get; set; } = null!;

    public virtual PhotoType PhotoTypeFkNavigation { get; set; } = null!;
}
