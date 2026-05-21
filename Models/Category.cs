using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ExhibitCategory> ExhibitCategories { get; set; } = new List<ExhibitCategory>();
}
