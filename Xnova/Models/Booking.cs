using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Booking
{
    public int Id { get; set; }

    public DateOnly? Date { get; set; }

    public int? Rating { get; set; }

    public string? Feedback { get; set; }

    public DateTime? CurrentDate { get; set; }

    public int? Status { get; set; }

    public int? UserId { get; set; }

    public int? FieldId { get; set; }

    public virtual ICollection<BookingSlot> BookingSlots { get; set; } = new List<BookingSlot>();

    public virtual Field? Field { get; set; }

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
