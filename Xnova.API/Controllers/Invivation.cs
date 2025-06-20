using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Invivation : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public Invivation(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // GET: api/Type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invitation>>> GetType()
        {
            return await _unitOfWork.InvitationRepository.GetAllAsync();
        }
        // GET: api/Type/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invitation>> GetType(int id)
        {
            var type = await _unitOfWork.InvitationRepository.GetByIdAsync(id);

            if (type == null)
            {
                return NotFound();
            }

            return type;
        }
    }
}
