using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class PhotoType
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ExhibitPhoto> ExhibitPhotos { get; set; } = new List<ExhibitPhoto>();
}
