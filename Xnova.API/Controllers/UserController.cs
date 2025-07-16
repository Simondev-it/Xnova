using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MimeKit;
using Xnova.API.RequestModel;
using Xnova.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly UnitOfWork _unitOfWork;
        private readonly EmailSettings _emailSettings;
        public UserController(
        IMemoryCache cache,
        UnitOfWork unitOfWork,
        IOptions<EmailSettings> emailSettings)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
            _emailSettings = emailSettings.Value;
        }
        // GET: api/User
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _unitOfWork.UserRepository.GetAllAsync();
        }

        // GET: api/User/5
        [HttpGet("GetIdAndName")]

        public async Task<ActionResult<IEnumerable<object>>> GetUserIdAndName()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            var userIdAndNames = users.Select(user => new
            {
                user.Id,
                user.Name,
                user.Image,
            }).ToList();

            return Ok(userIdAndNames);
        }
        [HttpGet("{id}")]
        [Authorize(Policy = "RequireUserOrAdminRole")]


        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        [HttpPost("register-request")]
        public async Task<IActionResult> RegisterWithOtp([FromBody] UserRegisterRequest request)
        {
            // Kiểm tra email đầu vào
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email không hợp lệ." });

            // Kiểm tra email người gửi trong cấu hình
            if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail) || string.IsNullOrWhiteSpace(_emailSettings.SenderPassword))
                return StatusCode(500, "Cấu hình email người gửi không hợp lệ.");

            // Kiểm tra trùng email
            var existingUser = await _unitOfWork.UserRepository.FindAsync(u => u.Email == request.Email);
            if (existingUser != null)
                return Conflict(new { message = "Email đã được sử dụng." });

            // Tạo mã OTP
            var otp = new Random().Next(100000, 999999).ToString();

            // Lưu tạm thông tin đăng ký + OTP
            _cache.Set(request.Email, new TempUserRegisterModel
            {
                Name = request.Name,
                Email = request.Email,
                Image = request.Image,
                Role = request.Role,
                Type = request.Type,
                Point = request.Point,
                PhoneNumber = request.PhoneNumber,
                Description = request.Description,
                Password = request.Password,
                Otp = otp
            }, TimeSpan.FromMinutes(5));

            // Soạn email OTP
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailSettings.SenderName ?? "Xnova", _emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(request.Email));
            email.Subject = "Mã xác thực OTP - Xnova";

            email.Body = new TextPart("html")
            {
                Text = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; border-radius: 10px; background-color: #f9f9f9;'>
                <h2 style='color: #1976d2;'>🔐 Mã OTP của bạn</h2>
                <p>Chào <strong>{request.Name}</strong>,</p>
                <p>Mã OTP để xác thực tài khoản của bạn là:</p>
                <h1 style='color: #d32f2f;'>{otp}</h1>
                <p>Mã có hiệu lực trong <strong>5 phút</strong>.</p>
                <br>
                <p>Trân trọng,</p>
                <p><strong>Xnova</strong></p>
            </div>"
            };

            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                await smtp.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return Ok(new { message = "Đã gửi OTP xác thực đến Gmail." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Không thể gửi email xác thực: " + ex.Message);
            }

        }


        // ✅ B2: Nhập OTP để xác thực và đăng ký thật
        [HttpPost("register-confirm")]
        public async Task<IActionResult> ConfirmOtpRegister([FromBody] ConfirmOtpModel model)
        {
            if (!_cache.TryGetValue(model.Email, out TempUserRegisterModel temp))
            {
                return BadRequest(new { message = "OTP đã hết hạn hoặc không tồn tại." });
            }

            if (temp.Otp != model.Otp)
            {
                return BadRequest(new { message = "OTP không chính xác." });
            }

            // Tạo user thật
            var user = new User
            {
                Name = temp.Name,
                Email = temp.Email,
                Password = temp.Password,
                PhoneNumber = temp.PhoneNumber,
                Role = temp.Role,
                Type = temp.Type,
                Description = temp.Description,
                Image = temp.Image,
                Point = 0,
                Status = 1
            };


            try
            {
                await _unitOfWork.UserRepository.CreateAsync(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Lỗi khi lưu người dùng.");
            }

            _cache.Remove(model.Email);

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        public class ConfirmOtpModel
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }


    }
}
