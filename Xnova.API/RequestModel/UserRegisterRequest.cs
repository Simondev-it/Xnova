namespace Xnova.API.RequestModel
{
    public class UserRegisterRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }

        // Optional fields (nếu bạn muốn cho nhập thêm):
        public string Role { get; set; } = "User";
        public string Type { get; set; } = "Normal";
        public string? Image { get; set; }
        public string? Description { get; set; }
        public int Point { get; set; } = 0; // ✅ thêm dòng này nếu bạn cần
    }
}
