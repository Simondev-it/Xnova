using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.API.RequestModel;
using Xnova.Models;
using Xnova.Repositories;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInvitationController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public UserInvitationController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInvitation>>> GetAllVouchers()
        {
            var vouchers = await _unitOfWork.userInvitationRepository.GetAllAsync();
            return Ok(vouchers);
        }

        
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
        [HttpPost]
        public async Task<IActionResult> CreateUserInvitation([FromBody] UserInvitationRequest request)
        {
            if (request == null)
                return BadRequest("Invalid data");

            var userInvitation = new UserInvitation
            {
                JoinDate = request.JoinDate,
                Status = request.Status,
                UserId = request.UserId,
                InvitationId = request.InvitationId
            };

            try
            {
                await _unitOfWork.userInvitationRepository.AddAsync(userInvitation);
                await _unitOfWork.userInvitationRepository.CompleteAsync();

                return Ok(new
                {
                    Message = "Invitation created successfully",
                    Data = userInvitation
                });
            }
            catch (Exception ex)    
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }

}
