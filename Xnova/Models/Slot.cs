using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Slot
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? Price { get; set; }

    public int? Status { get; set; }

    public int? FieldId { get; set; }

    public virtual ICollection<BookingSlot> BookingSlots { get; set; } = new List<BookingSlot>();

    public virtual Field? Field { get; set; }
}
