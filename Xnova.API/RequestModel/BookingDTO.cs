namespace Xnova.API.RequestModel
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public DateTime? CurrentDate { get; set; }
        public string Feedback { get; set; }
        public int? Rating { get; set; }
        public int PodId { get; set; }
        public int UserId { get; set; }
        public List<int> SlotIds { get; set; } // Chỉ cần ID của slot
    }
}
