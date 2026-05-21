using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ReceiptAct
{
    public int Id { get; set; }

    public DateOnly ActDate { get; set; }

    public int SourceFk { get; set; }

    public int ReceiptMethodFk { get; set; }

    public int ResponsiblePersonFk { get; set; }

    public virtual ICollection<Exhibit> Exhibits { get; set; } = new List<Exhibit>();

    public virtual ReceiptMethod ReceiptMethodFkNavigation { get; set; } = null!;

    public virtual Person ResponsiblePersonFkNavigation { get; set; } = null!;

    public virtual Person SourceFkNavigation { get; set; } = null!;
}
