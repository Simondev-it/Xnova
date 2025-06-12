using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class BookingSlot
{
    public int Id { get; set; }

    public int? BookingId { get; set; }

    public int? SlotId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Slot? Slot { get; set; }
}
