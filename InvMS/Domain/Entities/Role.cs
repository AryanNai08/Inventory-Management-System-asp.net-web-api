using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Role
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Privilege> Privileges { get; set; } = new List<Privilege>();

   
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
