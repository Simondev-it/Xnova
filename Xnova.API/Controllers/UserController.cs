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
        //[Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _unitOfWork.UserRepository.GetAllAsync();
        }
    }
}
