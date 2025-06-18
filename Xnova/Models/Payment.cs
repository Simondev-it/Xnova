using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Payment
{
    public int Id { get; set; }

    public string? Method { get; set; }

    public int? Amount { get; set; }

    public string? Note { get; set; }

    public DateTime? Date { get; set; }

    public string? Response { get; set; }


    public int? Status { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }
}
