namespace Xnova.API.RequestModel
{
    public class InvitationRequest
    {
        public string Name { get; set; }
        public int Booked { get; set; }
        public double JoiningCost { get; set; }
        public int TotalPlayer { get; set; }
        public int AvailablePlayer { get; set; }
        public string Standard { get; set; }
        public string KindOfSport { get; set; }
        public string Location { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public DateTime PostingDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }

        // Booking info lồng bên trong (nếu cần)
        public BookingRequest Booking { get; set; }
    }

    public class BookingRequest
    {
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string Feedback { get; set; }
        public DateTime CurrentDate { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
    }
}
