namespace Xnova.API.RequestModel
{
    public class SlotRequest
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public int? Price { get; set; }

        public int? Status { get; set; }

        public int? FieldId { get; set; }
    }
}
