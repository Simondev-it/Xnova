using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UnitOfWork _unitOfWork;

        public LoginController(IConfiguration configuration, UnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest loginRequest)
        {
            var user = await _unitOfWork.UserRepository.GetUserByCredentialsAsync(loginRequest.Email, loginRequest.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = GenerateJwtToken(user.Id, user.Email, user.Role); // Thêm Id vào hàm tạo token
            return Ok(new { token, user.Email, user.Role, user.Id });
        }

        private string GenerateJwtToken(int id, string username, string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt").Get<JWTSetting>();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username), // Email hoặc username của người dùng
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID
                new Claim(ClaimTypes.NameIdentifier, id.ToString()), // Chuyển ID thành string
                new Claim(ClaimTypes.Role, role) // Lưu role vào claims
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(jwtSettings.ExpireDays),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [HttpGet("admin")]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("You are an Admin and you can access this endpoint.");
        }

        // Endpoint yêu cầu role "User" để truy cập
        [HttpGet("user")]
        [Authorize(Policy = "RequireUserRole")]
        public IActionResult UserOnlyEndpoint()
        {
            return Ok("You are a User and you can access this endpoint.");
        }

        // Endpoint chỉ yêu cầu xác thực, không phân biệt role
        [HttpGet("authenticated")]
        [Authorize] // Chỉ yêu cầu token hợp lệ
        public IActionResult AuthenticatedEndpoint()
        {
            return Ok("You are authenticated and can access this endpoint.");
        }

    }
}
