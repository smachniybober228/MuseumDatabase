using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class ReceiptMethod
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<ReceiptAct> ReceiptActs { get; set; } = new List<ReceiptAct>();
}
