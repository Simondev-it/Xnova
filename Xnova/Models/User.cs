using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Image { get; set; }

    public string? Role { get; set; }

    public string? Description { get; set; }

    public string? PhoneNumber { get; set; }

    public int? Point { get; set; }

    public string? Type { get; set; }

    public int? Status { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Chatbox> Chatboxes { get; set; } = new List<Chatbox>();

    public virtual ICollection<FavoriteField> FavoriteFields { get; set; } = new List<FavoriteField>();

    public virtual ICollection<Friend> FriendFriendNavigations { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendUsers { get; set; } = new List<Friend>();

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<SaveField> SaveFields { get; set; } = new List<SaveField>();

    public virtual ICollection<UserInvitation> UserInvitations { get; set; } = new List<UserInvitation>();

    public virtual ICollection<UserVoucher> UserVouchers { get; set; } = new List<UserVoucher>();

    public virtual ICollection<Venue> Venues { get; set; } = new List<Venue>();
}
