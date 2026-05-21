using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ExpositionPlaceType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ExhibitOnExhibition> ExhibitOnExhibitions { get; set; } = new List<ExhibitOnExhibition>();
}
