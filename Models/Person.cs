using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class Person
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public string ContactEmail { get; set; } = null!;

    public int PersonTypeFk { get; set; }

    public virtual ICollection<Exhibition> Exhibitions { get; set; } = new List<Exhibition>();

    public virtual ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();

    public virtual PersonType PersonTypeFkNavigation { get; set; } = null!;

    public virtual ICollection<ReceiptAct> ReceiptActResponsiblePersonFkNavigations { get; set; } = new List<ReceiptAct>();

    public virtual ICollection<ReceiptAct> ReceiptActSourceFkNavigations { get; set; } = new List<ReceiptAct>();

    public virtual ICollection<RestorationOrderEntity> RestorationOrderEntityInitiatorFkNavigations { get; set; } = new List<RestorationOrderEntity>();

    public virtual ICollection<RestorationOrderEntity> RestorationOrderEntityRestorerFkNavigations { get; set; } = new List<RestorationOrderEntity>();
}
