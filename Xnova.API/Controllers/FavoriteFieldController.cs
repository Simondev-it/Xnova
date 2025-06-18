using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.API.RequestModel;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteFieldController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public FavoriteFieldController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoriteField>>> GetAllSlots()
        {
            var FF = await _unitOfWork.FavoriteFieldRepository.GetAllAsync(); // đúng
            return Ok(FF);
        }
        // GET: api/BookingOrder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FavoriteField>> GetBookingSlot(int id)
        {
            var FF = await _unitOfWork.FavoriteFieldRepository.GetByIdAsync(id);
            if (FF == null)
            {
                return NotFound();
            }
            return Ok(FF);
        }

        // POST: api/FavoriteField
        [HttpPost]
        public async Task<IActionResult> PostFavoriteField([FromBody] FavoriteFieldRequest request)
        {
            if (request.UserId == null || request.FieldId == null)
                return BadRequest("UserId và FieldId là bắt buộc.");

            var favorite = new FavoriteField
            {
                UserId = request.UserId,
                FieldId = request.FieldId,
                SetDate = request.SetDate ?? DateTime.Now
            };

            await _unitOfWork.FavoriteFieldRepository.CreateAsync(favorite);
            return Ok(favorite);
        }

        // PUT: api/FavoriteField/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFavoriteField(int id, [FromBody] FavoriteFieldRequest request)
        {
            var favorite = await _unitOfWork.FavoriteFieldRepository.GetAsync(f => f.Id == id);
            if (favorite == null)
                return NotFound();

            favorite.UserId = request.UserId ?? favorite.UserId;
            favorite.FieldId = request.FieldId ?? favorite.FieldId;
            favorite.SetDate = request.SetDate ?? favorite.SetDate;

            _unitOfWork.FavoriteFieldRepository.Update(favorite);
            await _unitOfWork.FavoriteFieldRepository.SaveAsync();

            return Ok(favorite);
        }

        // DELETE: api/FavoriteField/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavoriteField(int id)
        {
            var favorite = await _unitOfWork.FavoriteFieldRepository.GetAsync(f => f.Id == id);
            if (favorite == null)
                return NotFound();

            _unitOfWork.FavoriteFieldRepository.Remove(favorite);
            await _unitOfWork.FavoriteFieldRepository.SaveAsync();

            return Ok(new { message = "Xóa thành công." });
        }
    }
}
