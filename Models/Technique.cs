using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Technique
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ExhibitTechnique> ExhibitTechniques { get; set; } = new List<ExhibitTechnique>();
}
