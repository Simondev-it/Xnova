using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xnova;
using Xnova;
using Xnova.API.RequestModel;
using Xnova.Models;
using MailKit.Net.Smtp;
using MimeKit;

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
                // Lưu booking để có booking.Id
                await _unitOfWork.BookingRepository.CreateAsync(booking);
                await _unitOfWork.BookingRepository.SaveAsync();

                // Thêm dữ liệu vào bảng BookingSlot
                if (bookingRequest.SlotIds != null && bookingRequest.SlotIds.Any())
                {
                    foreach (var slotId in bookingRequest.SlotIds)
                    {
                        var bookingSlot = new BookingSlot
                        {
                            BookingId = booking.Id,
                            SlotId = slotId
                        };
                        await _unitOfWork.BookingSlotRepository.AddAsync(bookingSlot);
                    }
                    await _unitOfWork.BookingSlotRepository.SaveAsync();
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Lỗi khi tạo booking.", detail = ex.Message });
            }

            // Lấy thông tin user để gửi email
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == bookingRequest.UserId);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                string body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                  <meta charset='utf-8'>
                  <meta name='viewport' content='width=device-width, initial-scale=1'>
                </head>
                <body style='margin:0; padding:0; font-family:Arial, sans-serif; background-color:#f4f4f4;'>

                  <div style='width:100%; background-color:#f4f4f4; padding:40px 0;'>
                    <div style='max-width:700px; margin:0 auto; background:#fff; border-radius:10px; padding:30px; box-shadow:0 4px 12px rgba(0,0,0,0.1);'>

                      <h2 style='color:#2d89ef; text-align:center;'>✅ Xác nhận đặt chỗ thành công</h2>

                      <p>Chào <strong>{user.Name}</strong>,</p>

                      <p>Bạn đã đặt chỗ thành công với thông tin như sau:</p>

                      <table style='width:100%; margin-top:20px; margin-bottom:20px;'>
                        <tr>
                          <td style='padding:10px;'><strong>📅 Ngày đặt:</strong></td>
                          <td style='padding:10px;'>{booking.Date:dd/MM/yyyy}</td>
                        </tr>
                        <tr>
                          <td style='padding:10px;'><strong>🆔 Mã đơn:</strong></td>
                          <td style='padding:10px;'>#{booking.Id}</td>
                        </tr>
                      </table>

                      <p style='margin-top:20px;'>⏰ <strong>Vui lòng đến đúng giờ</strong> để không ảnh hưởng đến người khác.</p>

                      <p style='font-size:14px; color:#777;'>Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi!</p>
                      <p style='font-size:12px; color:#aaa;'>— Hệ thống đặt sân <strong>Xnova</strong></p>

                    </div>
                  </div>

                </body>
                </html>
    ";

                await SendEmailAsync(user.Email, "Xác nhận đặt chỗ", body);

                // Hẹn gửi mail nhắc nhở
                ScheduleReminder(user.Email, user.Name, booking.Date.Value.ToDateTime(TimeOnly.MinValue));
            }

            // Truy vấn lại booking có bao gồm Field + Slot
            var fullBooking = await _unitOfWork.BookingRepository.GetAsync(
                b => b.Id == booking.Id,
                includeProperties: "Field,BookingSlots.Slot"
            );

            // Trả về dữ liệu đã tạo đầy đủ
            return CreatedAtAction(nameof(GetBookingById), new { id = fullBooking.Id }, fullBooking);
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

        // Send mail 

        private async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("duongntse180440@fpt.edu.vn")); // Thay bằng email thật
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("duongntse180440@fpt.edu.vn", "fsof gkfp glgf bscu"); // Dùng App Password
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private void ScheduleReminder(string to, string name, DateTime bookingTime)
        {
            var reminderTime = bookingTime.AddMinutes(-10);
            var delay = reminderTime - DateTime.Now;

            if (delay.TotalMilliseconds > 0)
            {
                _ = Task.Run(async () =>
                {
                    await Task.Delay(delay);
                    string reminderBody = $@"
                      <div style='font-family: Arial; width: 100%; box-sizing: border-box; border: 1px solid #ffc107; padding: 20px; border-radius: 10px; background-color: #fff8e1;'>
                        <h2 style='color: #ff9800;'>Nhắc nhở lịch đặt sân</h2>
                        <p>Chào <strong>{name}</strong>,</p>
                        <p>Hệ thống nhắc bạn rằng bạn có lịch đặt sân vào ngày <strong>{bookingTime:dd/MM/yyyy}</strong>.</p>
                        <p>Vui lòng đến đúng giờ để không ảnh hưởng đến thời gian sân của bạn và người khác.</p>
                        <p style='margin-top: 20px;'>Chúc bạn có trận đấu thật vui vẻ!</p>
                        <p style='font-size: 12px; color: gray;'>— Đội ngũ Xnova</p>
                      </div>
                    ";
                    await SendEmailAsync(to, "⏰ Nhắc nhở lịch đặt sân", reminderBody);

                });
            }   
        }

    }
}
