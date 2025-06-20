using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Field
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? Status { get; set; }

    public int? TypeId { get; set; }

    public int? VenueId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<FavoriteField> FavoriteFields { get; set; } = new List<FavoriteField>();

    public virtual ICollection<SaveField> SaveFields { get; set; } = new List<SaveField>();

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    public virtual Type? Type { get; set; }

    public virtual Venue? Venue { get; set; }
}
