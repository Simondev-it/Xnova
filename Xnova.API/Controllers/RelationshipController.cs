using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationshipController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public RelationshipController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // GET: api/Type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Relationship>>> GetType()
        {
            return await _unitOfWork.relationshipRepository.GetAllAsync();
        }
        // GET: api/Type/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Relationship>> GetType(int id)
        {
            var type = await _unitOfWork.relationshipRepository.GetByIdAsync(id);

            if (type == null)
            {
                return NotFound();
            }

            return type;
        }
    }
}
