using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;
using Type = Xnova.Models.Type;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypeController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public TypeController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // GET: api/Type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Type>>> GetType()
        {
            return await _unitOfWork.TypeRepository.GetAllAsync();
        }
        // GET: api/Type/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Type>> GetType(int id)
        {
            var type = await _unitOfWork.TypeRepository.GetByIdAsync(id);

            if (type == null)
            {
                return NotFound();
            }

            return type;
        }
    }
}
