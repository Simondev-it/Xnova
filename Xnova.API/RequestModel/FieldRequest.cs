namespace Xnova.API.RequestModel
{
    public class FieldRequest
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int? Status { get; set; }

        public int? TypeId { get; set; }

        public int? VenueId { get; set; }
    }
}
