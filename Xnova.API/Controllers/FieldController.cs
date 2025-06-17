using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FieldController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public FieldController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Field>>> GetAllSlots()
        {

            var Fields = await _unitOfWork.FieldRepository.GetAllAsync(); // đúng
            return Ok(Fields);

        }
        // GET: api/BookingOrder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Field>> GetBookingSlot(int id)
        {
            var Fields = await _unitOfWork.FieldRepository.GetByIdAsync(id);
            if (Fields == null)
            {
                return NotFound();
            }
            return Ok(Fields);
        }

    }
}
