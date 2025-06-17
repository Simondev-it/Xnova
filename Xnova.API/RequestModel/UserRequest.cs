namespace Xnova.API.RequestModel
{
    public class UserRequest
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Image { get; set; }

        public string? Role { get; set; }

        public string? Description { get; set; }

        public string? PhoneNumber { get; set; }

        public int? Point { get; set; }

        public string? Type { get; set; }

        public int? Status { get; set; }
    }
}
