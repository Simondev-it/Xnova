using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Invitation
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public int? Booked { get; set; }

    public int? JoiningCost { get; set; }

    public int? TotalPlayer { get; set; }

    public int? AvailablePlayer { get; set; }

    public string? Standard { get; set; }

    public string? KindOfSport { get; set; }

    public string? Location { get; set; }

    public string? Longitude { get; set; }

    public string? Latitude { get; set; }

    public DateOnly? Date { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public DateOnly? PostingDate { get; set; }

    public int? Status { get; set; }

    public int? UserId { get; set; }

    public int? BookingId { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserInvitation> UserInvitations { get; set; } = new List<UserInvitation>();
}
