using Xnova.Models;

namespace Xnova.API.RequestModel
{
    public class UserInvitationRequest
    {
        //public int Id { get; set; }

        public DateTime? JoinDate { get; set; }

        public int? Status { get; set; }

        public int? UserId { get; set; }

        public int? InvitationId { get; set; }

        //public virtual Invitation? Invitation { get; set; }

        //public virtual User? User { get; set; }
    }
}
