using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.DTO;
using Project_2025_Web.DTOs;
using Project_2025_Web.Services;
namespace Project_2025_Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [Consumes("application/json")]
[Produces("application/json")]
    
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly IPermissionService _permissionService;
        public ReservationsController(IReservationService reservationService, IPermissionService permissionService)
        {
            _reservationService = reservationService;
            _permissionService = permissionService;
        }
        // GET: api/reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservationDTO>>> GetReservations(
            [FromQuery] string? status = null,
            [FromQuery] int? userId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null)
        {
            var filter = new ReservationFilterDTO
            {
                Status = status,
                UserId = userId,
                DateFrom = dateFrom,
                DateTo = dateTo
            };
            var reservations = await _reservationService.GetFilteredAsync(filter);
            return Ok(reservations);
        }
        // GET: api/reservations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationDTO>> GetReservation(int id)
        {
            var response = await _reservationService.GetOneAsync(id);
            if (!response.IsSucess || response.Result == null)
            {
                return NotFound();
            }
            return Ok(response.Result);
        }
        // POST: api/reservations
        [HttpPost]
        [AuthorizePermission("Hacer Reservas")]
        public async Task<ActionResult<ReservationDTO>> CreateReservation(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }
            dto.Id_User = userId;
            var response = await _reservationService.CreateAsync(dto);
            if (!response.IsSucess)
            {
                return BadRequest(response.Message);
            }
            return CreatedAtAction(nameof(GetReservation), new { id = response.Result.Id }, response.Result);
        }
        // PUT: api/reservations/{id}
        [HttpPut("{id}")]
        [AuthorizePermission("Gestionar Reservas")]
        public async Task<IActionResult> UpdateReservation(int id, ReservationDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }
            var response = await _reservationService.EditAsync(dto);
            if (!response.IsSucess)
            {
                return BadRequest(response.Message);
            }
            return NoContent();
        }
        // DELETE: api/reservations/{id}
        [HttpDelete("{id}")]
        [AuthorizePermission("Gestionar Reservas")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var response = await _reservationService.DeleteAsync(id);
            if (!response.IsSucess)
            {
                return BadRequest(response.Message);
            }
            return NoContent();
        }
    }
}