using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Relationship
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Friend> Friends { get; set; } = new List<Friend>();
}
