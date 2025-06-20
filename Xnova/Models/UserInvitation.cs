using System;
using System.Collections.Generic;

namespace Xnova.Models;

public partial class UserInvitation
{
    public int Id { get; set; }

    public DateTime? JoinDate { get; set; }

    public int? Status { get; set; }

    public int? UserId { get; set; }

    public int? InvitationId { get; set; }

    public virtual Invitation? Invitation { get; set; }

    public virtual User? User { get; set; }
}
