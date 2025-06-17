using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlotController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public SlotController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Slot>>> GetAllSlots()
        {

            var Slot = await _unitOfWork.SlotRepository.GetAllAsync(); // đúng
            return Ok(Slot);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Slot>> GetBookingSlot(int id)
        {
            var Slot = await _unitOfWork.SlotRepository.GetByIdAsync(id);
            if (Slot == null)
            {
                return NotFound();
            }

            return Ok(Slot);
        }
    }
}
