using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class RoleEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<PersonRole> PersonRoles { get; set; } = new List<PersonRole>();
}
