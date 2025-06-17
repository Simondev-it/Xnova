using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VenueController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public VenueController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // GET: api/User
        [HttpGet]
        //[Authorize(Policy = "RequireAdminRole")]
        public async Task<ActionResult<IEnumerable<Venue>>> GetUser()
        {
            return await _unitOfWork.VenueRepository.GetAllAsync();
        }
    }
}
