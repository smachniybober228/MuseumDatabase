using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Museum.Models;

public partial class RestorationOrderEntity
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime ReceiptDate { get; set; }

    public string ReasonDirection { get; set; } = null!;

    public DateTime PlannedCompletionDate { get; set; }

    public int ExhibitFk { get; set; }

    public int RestorerFk { get; set; }

    public int InitiatorFk { get; set; }

    public virtual Exhibit ExhibitFkNavigation { get; set; } = null!;

    public virtual Person InitiatorFkNavigation { get; set; } = null!;

    public virtual ICollection<RequiredWorkType> RequiredWorkTypes { get; set; } = new List<RequiredWorkType>();

    public virtual ICollection<RestorationAct> RestorationActs { get; set; } = new List<RestorationAct>();

    public virtual Person RestorerFkNavigation { get; set; } = null!;

    public virtual ICollection<ReturnAct> ReturnActs { get; set; } = new List<ReturnAct>();

    public virtual ICollection<WorkLogEntry> WorkLogEntries { get; set; } = new List<WorkLogEntry>();
}
