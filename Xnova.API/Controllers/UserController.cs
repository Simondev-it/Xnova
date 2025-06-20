using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("GetIDandName")]

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
    }
}
