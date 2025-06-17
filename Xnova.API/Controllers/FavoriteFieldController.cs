using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
