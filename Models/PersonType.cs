using System;
using System.Collections.Generic;

namespace Museum.Models;

public partial class PersonType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Person> People { get; set; } = new List<Person>();
}
