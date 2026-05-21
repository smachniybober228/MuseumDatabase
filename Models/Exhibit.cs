using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Exhibit
{
    public int Id { get; set; }

    public string InventoryNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public double LengthCm { get; set; }

    public double WidthCm { get; set; }

    public double HeightCm { get; set; }

    public string CreationDate { get; set; } = null!;

    public string Author { get; set; } = null!;

    public string OriginPlace { get; set; } = null!;

    public int ExhibitStatusFk { get; set; }

    public int ReceiptActFk { get; set; }

    public virtual ICollection<ExhibitCategory> ExhibitCategories { get; set; } = new List<ExhibitCategory>();

    public virtual ICollection<ExhibitMaterial> ExhibitMaterials { get; set; } = new List<ExhibitMaterial>();

    public virtual ICollection<ExhibitOnExhibition> ExhibitOnExhibitions { get; set; } = new List<ExhibitOnExhibition>();

    public virtual ICollection<ExhibitPhoto> ExhibitPhotos { get; set; } = new List<ExhibitPhoto>();

    public virtual ExhibitStatus ExhibitStatusFkNavigation { get; set; } = null!;

    public virtual ICollection<ExhibitTechnique> ExhibitTechniques { get; set; } = new List<ExhibitTechnique>();

    public virtual ReceiptAct ReceiptActFkNavigation { get; set; } = null!;

    public virtual ICollection<RestorationOrderEntity> RestorationOrderEntities { get; set; } = new List<RestorationOrderEntity>();

    public virtual ICollection<WriteOffAct> WriteOffActs { get; set; } = new List<WriteOffAct>();
}
