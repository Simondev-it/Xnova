using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveFieldController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public SaveFieldController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // GET: api/Type
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaveField>>> GetType()
        {
            return await _unitOfWork.saveFieldRepository.GetAllAsync();
        }
        // GET: api/Type/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SaveField>> GetType(int id)
        {
            var type = await _unitOfWork.saveFieldRepository.GetByIdAsync(id);

            if (type == null)
            {
                return NotFound();
            }

            return type;
        }
    }
}
