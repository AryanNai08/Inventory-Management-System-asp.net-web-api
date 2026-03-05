using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Privilege
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
