using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;
using Xnova.Repositories;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInvivationController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public UserInvivationController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // GET: api/UserVoucher
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInvitation>>> GetAllVouchers()
        {
            var vouchers = await _unitOfWork.userInvitationRepository.GetAllAsync();
            return Ok(vouchers);
        }

        // GET: api/UserVoucher/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInvitation>> GetVoucherById(int id)
        {
            var voucher = await _unitOfWork.userInvitationRepository.GetByIdAsync(id);

            if (voucher == null)
            {
                return NotFound(new { message = "Không tìm thấy voucher." });
            }

            return Ok(voucher);
        }
    }

}
