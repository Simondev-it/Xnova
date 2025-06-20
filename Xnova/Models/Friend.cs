using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class Friend
{
    public int Id { get; set; }

    public int? FriendId { get; set; }

    public int? UserId { get; set; }

    public int? RelationshipId { get; set; }

    public virtual User? FriendNavigation { get; set; }

    public virtual Relationship? Relationship { get; set; }

    public virtual User? User { get; set; }
}
