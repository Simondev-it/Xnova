using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.API.RequestModel;
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
        // POST: api/Field
        [HttpPost]
        public async Task<IActionResult> PostField(FieldRequest request)
        {
            var field = new Field
            {
                Name = request.Name,
                Description = request.Description,
                Status = request.Status,
                TypeId = request.TypeId,
                VenueId = request.VenueId
            };

            await _unitOfWork.FieldRepository.CreateAsync(field);
            await _unitOfWork.FieldRepository.SaveAsync();

            return Ok(field);
        }

        // PUT: api/Field/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutField(int id, FieldRequest request)
        {
            var field = await _unitOfWork.FieldRepository.GetAsync(f => f.Id == id);
            if (field == null) return NotFound();

            field.Name = request.Name;
            field.Description = request.Description;
            field.Status = request.Status;
            field.TypeId = request.TypeId;
            field.VenueId = request.VenueId;

            _unitOfWork.FieldRepository.Update(field);
            await _unitOfWork.FieldRepository.SaveAsync();

            return Ok(field);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteField(int id)
        {
            var field = await _unitOfWork.FieldRepository.GetAsync(
                f => f.Id == id,
                includeProperties: "Slots"
            );

            if (field == null)
                return NotFound("Không tìm thấy sân với ID đã cho.");

            // Xóa toàn bộ các Slot liên quan
            foreach (var slot in field.Slots.ToList())
            {
                _unitOfWork.SlotRepository.Remove(slot);
            }

            // Xóa Field
            _unitOfWork.FieldRepository.Remove(field);
            await _unitOfWork.FieldRepository.SaveAsync();

            return Ok("Đã xóa sân và các Slot liên quan.");
        }


    }
}
