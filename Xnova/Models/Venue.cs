using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Venue
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Contact { get; set; }

    public int? Status { get; set; }

    public int? UserId { get; set; }
    public string ? Longitude { get; set; } 

    public string? Latitude { get; set; }

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual User? User { get; set; }
}
