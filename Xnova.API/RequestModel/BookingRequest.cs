namespace Xnova.API.RequestModel
{
    public class BookingRequest
    {
        public int Id { get; set; }

        public DateOnly? Date { get; set; }

        public int? Rating { get; set; }

        public string? Feedback { get; set; }

        public DateTime? CurrentDate { get; set; }

        public int? Status { get; set; }

        public int? UserId { get; set; }

        public int? FieldId { get; set; }

        public virtual ICollection<int> SlotIds { get; set; } = new List<int>();
    }
}
