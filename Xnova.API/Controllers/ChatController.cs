using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Xnova.API.RequestModel;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ChatController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("ask")]
        [AllowAnonymous]
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            // Lấy userId từ token JWT (nếu có)
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("id")?.Value;

            // Lấy sessionId từ request (nếu có, cho guest)
            var sessionId = request.SessionId;

            // Gọi AskAsync với userId hoặc sessionId
            var (reply, newSessionId) = await _unitOfWork.ChatRepository.AskAsync(request.Message, userId ?? sessionId);

            // Trả về phản hồi và sessionId (nếu là guest)
            return Ok(new { reply, sessionId = newSessionId ?? sessionId });
        }

        [HttpGet("history")]
        public IActionResult GetChatHistory(string? sessionId = null)
        {
            // Lấy userId từ token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("id")?.Value;

            // Sử dụng sessionId nếu không có userId
            var id = userId ?? sessionId;

            if (string.IsNullOrEmpty(id))
                return Ok(new { Message = "Người dùng guest không có lịch sử trò chuyện hoặc cần cung cấp sessionId." });

            var history = _unitOfWork.ChatRepository.GetHistory(id);
            return Ok(history.Select(h => new { h.Question, h.Answer }));
        }

        [HttpGet("greeting")]
        [AllowAnonymous]
        public IActionResult GetGreeting()
        {
            var greeting = _unitOfWork.ChatRepository.GetGreeting();
            return Ok(new { greeting });
        }
    }
}