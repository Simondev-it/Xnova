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
        public async Task<IActionResult> Ask([FromBody] ChatRequest request)
        {
            // 🔑 Lấy userId từ token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không xác định được người dùng.");

            // 📩 Gửi câu hỏi và userId vào ChatRepository
            var reply = await _unitOfWork.ChatRepository.AskAsync(request.Message, userId);
            return Ok(new { reply });
        }

        [HttpGet("history")]
        public IActionResult GetChatHistory()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                         User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Không xác định được người dùng.");

            var history = _unitOfWork.ChatRepository.GetHistory(userId);
            return Ok(history.Select(h => new { h.Question, h.Answer }));
        }
    }
}
