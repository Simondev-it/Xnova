using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xnova.API.RequestModel;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public UserController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

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
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserRequest userRequest)
        {
            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _unitOfWork.UserRepository
                .FindAsync(u => u.Email == userRequest.Email);

            if (existingUser != null)
            {
                return Conflict(new { message = "Email đã được sử dụng." }); // HTTP 409
            }

            var user = new User
            {
                Id = userRequest.Id,
                Name = userRequest.Name,
                Email = userRequest.Email,
                Image = userRequest.Image,
                Role = userRequest.Role,
                Type = userRequest.Type,
                Point = userRequest.Point,
                PhoneNumber = userRequest.PhoneNumber,
                Description = userRequest.Description,
                Password = userRequest.Password,
            };

            try
            {
                await _unitOfWork.UserRepository.CreateAsync(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Lỗi khi lưu người dùng.");
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

    }
}
