using Xnova.Models;

namespace Xnova.API.RequestModel
{
    public class BookingSlotRequest
    {
        public int Id { get; set; }

        public int? BookingId { get; set; }

        public int? SlotId { get; set; }

        //public virtual Booking? Booking { get; set; }

        //public virtual Slot? Slot { get; set; }
    }
}
