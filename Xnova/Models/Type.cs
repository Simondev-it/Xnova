using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Type
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();
}
