using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xnova.Models;
using Xnova.Repositories;
using Xnova.API.RequestModel;

namespace Xnova.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvitationController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public InvitationController(UnitOfWork unitOfWork) => _unitOfWork = unitOfWork;


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Invitation>>> GetType()
        {
            return await _unitOfWork.InvitationRepository.GetAllAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Invitation>> GetType(int id)
        {
            var type = await _unitOfWork.InvitationRepository.GetByIdAsync(id);

            if (type == null)
            {
                return NotFound();
            }

            return type;
        }
        [HttpPost]
        public async Task<ActionResult<Invitation>> CreateInvitation([FromBody] InvitationRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invitation is null.");
            }
            var invitation = new Invitation
            {
                Name = request.Name,
                Booked = request.Booked,
                JoiningCost = (int)request.JoiningCost,
                TotalPlayer = request.TotalPlayer,
                AvailablePlayer = request.AvailablePlayer,
                Standard = request.Standard,
                KindOfSport = request.KindOfSport,
                Location = request.Location,
                Longitude = request.Longitude,
                Latitude = request.Latitude,
                Date = DateOnly.FromDateTime(request.Date),

                StartTime = TimeOnly.Parse(request.StartTime),
                EndTime = TimeOnly.Parse(request.EndTime),

                PostingDate = DateOnly.FromDateTime(request.PostingDate),

                Status = (int)request.Status,
                UserId = request.UserId,
                BookingId = request.BookingId,
                //Booking = new Booking
                //{
                //    Date = request.Booking.Date,
                //    Rating = request.Booking.Rating,
                //    Feedback = request.Booking.Feedback,
                //    CurrentDate = request.Booking.CurrentDate,
                //    Status = request.Booking.Status,
                //    UserId = request.Booking.UserId
                //}
            };

            await _unitOfWork.InvitationRepository.AddAsync(invitation);
            await _unitOfWork.InvitationRepository.CompleteAsync();

            // Trả về 201 Created kèm theo route đến invitation vừa tạo
            return CreatedAtAction(nameof(GetType), new { id = invitation.Id }, invitation);
        }
    }
}
