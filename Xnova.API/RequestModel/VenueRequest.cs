namespace Xnova.API.RequestModel
{
    public class VenueRequest
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Address { get; set; }

        public string? Contact { get; set; }

        public int? Status { get; set; }

        public int? UserId { get; set; }
    }
}
