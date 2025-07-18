namespace Xnova.API.RequestModel
{
    public class ChatRequest
    {
        public string Message { get; set; }
        public string? SessionId { get; set; } // Thêm để theo dõi phiên guest
    }
}
