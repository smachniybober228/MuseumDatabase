using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Material
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ExhibitMaterial> ExhibitMaterials { get; set; } = new List<ExhibitMaterial>();
}
