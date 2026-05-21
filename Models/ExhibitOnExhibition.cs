using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExhibitOnExhibition
{
    public int Id { get; set; }

    public string PlaceIdentifier { get; set; } = null!;

    public string LabelData { get; set; } = null!;

    public int ExhibitionFk { get; set; }

    public int ExhibitFk { get; set; }

    public int ExpositionPlaceTypeFk { get; set; }

    public virtual Exhibit ExhibitFkNavigation { get; set; } = null!;

    public virtual Exhibition ExhibitionFkNavigation { get; set; } = null!;

    public virtual ExpositionPlaceType ExpositionPlaceTypeFkNavigation { get; set; } = null!;
}
