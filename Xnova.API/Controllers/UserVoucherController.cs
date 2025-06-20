using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVoucherController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public UserVoucherController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // GET: api/UserVoucher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserVoucher>>> GetAllVouchers()
        {
            var vouchers = await _unitOfWork.userVoucherRepository.GetAllAsync();
            return Ok(vouchers);
        }

        // GET: api/UserVoucher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserVoucher>> GetVoucherById(int id)
        {
            var voucher = await _unitOfWork.userVoucherRepository.GetByIdAsync(id);

            if (voucher == null)
            {
                return NotFound(new { message = "Không tìm thấy voucher." });
            }

            return Ok(voucher);
        }
    }

}
