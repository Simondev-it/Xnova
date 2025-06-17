using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingSlotController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        public BookingSlotController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingSlot>>> GetAllSlots()
        {
            var BoS = await _unitOfWork.BookingSlotRepository.GetAllAsync(); // đúng
            return Ok(BoS);
        }
        // GET: api/BookingOrder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingSlot>> GetBookingSlot(int id)
        {
            var BoS = await _unitOfWork.BookingSlotRepository.GetByIdAsync(id);
            if (BoS == null)
            {
                return NotFound();
            }
            return Ok(BoS);
        }
    }
}
