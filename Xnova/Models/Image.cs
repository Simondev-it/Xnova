using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Image
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Link { get; set; }

    public int? Status { get; set; }

    public int? VenueId { get; set; }

    public virtual Venue? Venue { get; set; }
}
