using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.API.RequestModel;
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

        [HttpPost]
        public async Task<ActionResult<BookingSlot>> PostBookingSlot(BookingSlotRequest request)
        {
            if (request.BookingId == null || request.SlotId == null)
            {
                return BadRequest(new { message = "BookingId và SlotId là bắt buộc." });
            }

            var bookingSlot = new BookingSlot
            {
                BookingId = request.BookingId,
                SlotId = request.SlotId
            };

            await _unitOfWork.BookingSlotRepository.CreateAsync(bookingSlot);

            return CreatedAtAction(nameof(GetBookingSlotById), new { id = bookingSlot.Id }, bookingSlot);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutBookingSlot(int id, BookingSlotRequest request)
        {
            var existing = await _unitOfWork.BookingSlotRepository.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Không tìm thấy booking slot." });

            existing.BookingId = request.BookingId;
            existing.SlotId = request.SlotId;

            await _unitOfWork.BookingSlotRepository.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBookingSlot(int id)
        {
            var bookingSlot = await _unitOfWork.BookingSlotRepository.GetByIdAsync(id);
            if (bookingSlot == null)
                return NotFound(new { message = "Không tìm thấy booking slot." });

            await _unitOfWork.BookingSlotRepository.RemoveAsync(bookingSlot);
            return NoContent();
        }

        [HttpGet("detail/{id}")]
        public async Task<ActionResult<BookingSlot>> GetBookingSlotById(int id)
        {
            var result = await _unitOfWork.BookingSlotRepository.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return result;
        }




    }
}
