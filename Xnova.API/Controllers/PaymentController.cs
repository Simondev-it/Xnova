using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xnova.API.RequestModel;
using Xnova.API.Services;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IVnpayService _vpnpayService;
        public PaymentController(UnitOfWork unitOfWork, IVnpayService vnpayService)
        {
            _unitOfWork = unitOfWork;
            _vpnpayService = vnpayService;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payment>>> GetAllSlots()
        {

            var payment = await _unitOfWork.PaymentRepository.GetAllAsync(); // đúng
            return Ok(payment);

        }
        [HttpGet("booking/{bookingId}")]
        public async Task<ActionResult<Payment>> GetPaymentByBookingId(int bookingId)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByBookingIdAsync(bookingId);

            if (payment == null)
            {
                return NotFound(new { Message = "Payment not found for the given BookingId." });
            }

            return Ok(payment);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Payment>> GetBookingSlot(int id)
        {
            var payment = await _unitOfWork.PaymentRepository.GetByIdAsync(id);
            if (payment == null)
            {
                return NotFound();
            }
            return Ok(payment);
        }
        //Vnpay


        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] VnPaymentRequestModel model)
        {
            if (model == null)
            {
                return BadRequest("Request cannot be null.");
            }
            // Tạo đối tượng Payment và lưu vào cơ sở dữ liệu
            var utcDate = DateTime.UtcNow;
            var payment = new Payment
            {
                //Id = model.Id,
                Method = "Thanh toán qua VNPay",
                Amount = model.Amount, // Kiểm tra kiểu dữ liệu
                Date = utcDate,
                Response = "Chưa thanh toán", // Đánh dấu trạng thái ban đầu là 'Pending'
                BookingId = model.OrderId , // Dùng OrderId từ model
                Status = 0 ,
                Note = "Pay with VNPay method"
            };
            try
            {
                await _unitOfWork.PaymentRepository.CreateAsync(payment);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new
                {
                    Message = "Có lỗi xảy ra khi lưu thông tin thanh toán.",
                    Error = dbEx.InnerException?.Message ?? dbEx.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = "Có lỗi xảy ra khi lưu thông tin thanh toán.",
                    Error = ex.InnerException?.Message ?? ex.Message
                });
            }
            // Tạo URL thanh toán thông qua VNPay
            var paymentUrl = _vpnpayService.CreatePaymentUrl(HttpContext, model);

            return Ok(new { PaymentUrl = paymentUrl, PaymentId = payment.Id });
        }
        [HttpGet("PaymentCallBack")]
        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vpnpayService.PaymentExecute(Request.Query);

            if (response == null || response.VnPayResponsecode != "00")
            {
                string FailUrl = $"http://localhost:5173?message={Uri.EscapeDataString("Thanh toán không thành công")}";
                return Redirect(FailUrl);
            }

            var payment = await _unitOfWork.PaymentRepository.GetByBookingIdAsync(response.OrderId);
            if (payment == null)
            {
                return NotFound("Không tìm thấy đơn hàng thanh toán.");
            }

            // ✅ Đảm bảo Date luôn là UTC
            if (payment.Date.HasValue && payment.Date.Value.Kind != DateTimeKind.Utc)
            {
                payment.Date = DateTime.SpecifyKind(payment.Date.Value, DateTimeKind.Utc);
            }

            Console.WriteLine("===> Trước khi cập nhật:");
            Console.WriteLine($"BookingId: {response.OrderId}");
            Console.WriteLine($"Response cũ: {payment.Response}");

            // ✅ Cập nhật trạng thái thanh toán
            payment.Response = "Đã thanh toán";
            payment.Status = 1;

            await _unitOfWork.PaymentRepository.UpdateAsync1(payment);

            string successUrl = $"https://localhost:7226/swagger/index.html?message={Uri.EscapeDataString("Thanh toán thành công")}";
            return Redirect(successUrl);
        }


        
        [HttpGet("PaymentResult")]
        public IActionResult PaymentResult(string message)
        {
            return Ok(new { Message = message });
        }

    }
}
