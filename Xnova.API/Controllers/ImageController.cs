using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public ImageController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Image>>> GetAllSlots()
        {

            var Image = await _unitOfWork.ImageRepository.GetAllAsync(); // đúng
            return Ok(Image);

        }
        // GET: api/BookingOrder/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetBookingSlot(int id)
        {
            var Image = await _unitOfWork.ImageRepository.GetByIdAsync(id);
            if (Image == null)
            {
                return NotFound();
            }
            return Ok(Image);
        }
    }
}
