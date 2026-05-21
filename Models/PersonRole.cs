using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class PersonRole
{
    public int Id { get; set; }

    public int PersonFk { get; set; }

    public int RoleFk { get; set; }

    public virtual Person PersonFkNavigation { get; set; } = null!;

    public virtual RoleEntity RoleFkNavigation { get; set; } = null!;
}
