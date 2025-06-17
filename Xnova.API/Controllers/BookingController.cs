using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xnova;
using Xnova;
using Xnova.API.RequestModel;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public BookingController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;
        
        // GET: api/Booking
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBooking()
        {
            return await _unitOfWork.BookingRepository.GetAllAsync();
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }
        // POST: api/bookings
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(BookingRequest bookingRequest)
        {
            // Tạo đối tượng Booking từ dữ liệu request
            var booking = new Booking
            {
                Date = bookingRequest.Date,
                Rating = bookingRequest.Rating,
                Feedback = bookingRequest.Feedback,
                CurrentDate = bookingRequest.CurrentDate,
                Status = bookingRequest.Status,
                UserId = bookingRequest.UserId,
                FieldId = bookingRequest.FieldId
            };

            try
            {
                await _unitOfWork.BookingRepository.CreateAsync(booking);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo booking.", detail = ex.Message });
            }

            // Trả về dữ liệu vừa tạo
            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }

        //StrongP@ssw0rd


        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, BookingRequest bookingRequest)
        {
            // Kiểm tra tồn tại booking
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking không tồn tại." });
            }

            // Cập nhật các thuộc tính
            booking.Date = bookingRequest.Date;
            booking.Rating = bookingRequest.Rating;
            booking.Feedback = bookingRequest.Feedback;
            booking.CurrentDate = bookingRequest.CurrentDate;
            booking.Status = bookingRequest.Status;
            booking.UserId = bookingRequest.UserId;
            booking.FieldId = bookingRequest.FieldId;

            try
            {
                await _unitOfWork.BookingRepository.UpdateAsync(booking);
            }
            catch (DbUpdateConcurrencyException)
            {
                var exists = await _unitOfWork.BookingRepository.GetByIdAsync(id);
                if (exists == null)
                {
                    return NotFound(new { message = "Booking đã bị xóa hoặc không còn tồn tại." });
                }

                return StatusCode(500, new { message = "Lỗi khi cập nhật Booking." });
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking không tồn tại." });
            }

            // Xóa các booking slot liên quan
            var bookingSlots = await _unitOfWork.BookingSlotRepository.GetAllAsync(bs => bs.BookingId == id);
            _unitOfWork.BookingSlotRepository.RemoveRange(bookingSlots);

            // Xóa các thanh toán liên quan (nếu có)
            var payments = await _unitOfWork.PaymentRepository.GetAllAsync(p => p.BookingId == id);
            _unitOfWork.PaymentRepository.RemoveRange(payments);

            // Xóa booking
            await _unitOfWork.BookingRepository.RemoveAsync(booking);

            return NoContent();
        }










        [HttpGet("detail/{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetByIdAsync(id);
            if (booking == null)
            {
                return NotFound(new { message = "Booking không tồn tại." });
            }

            return booking;
        }


    }
}
