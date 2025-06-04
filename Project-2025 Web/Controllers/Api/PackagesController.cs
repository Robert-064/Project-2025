using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Services;
using Project_2025_Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Project_2025_Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackagesController : ControllerBase
    {
        private readonly IPlanService _packageService;
        private readonly IPermissionService _permissionService;
        public PackagesController(IPlanService packageService, IPermissionService permissionService)
        {
            _packageService = packageService;
            _permissionService = permissionService;
        }
        // GET: api/packages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlanDTO>>> GetPackages([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var packages = await _packageService.GetPlansPagedAsync(page, pageSize);
            var totalCount = await _packageService.GetPlansCountAsync();
            Response.Headers.Add("X-Total-Count", totalCount.ToString());
            Response.Headers.Add("X-Page-Size", pageSize.ToString());
            Response.Headers.Add("X-Current-Page", page.ToString());
            var dtos = packages.Select(p => new PlanDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Basic_Price = p.Basic_Price,
                Type_Difficulty = p.Type_Difficulty,
                Max_Persons = p.Max_Persons,
                Distance = p.Distance,
                ImagePath1 = p.ImageUrl1,
                ImagePath2 = p.ImageUrl2
            });
            return Ok(dtos);
        }
        // GET: api/packages/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PlanDTO>> GetPackage(int id)
        {
            var package = await _packageService.GetPackageByIdAsync(id);
            if (package == null)
            {
                return NotFound();
            }
            return Ok(package);
        }
        // POST: api/packages
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PlanDTO>> CreatePackage([FromForm] PlanDTO dto)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }
            var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "Gestionar Paquetes");
            if (!hasPermission)
            {
                return Forbid();
            }
            var response = await _packageService.CreateAsync(dto);
            if (!response.IsSucess)
            {
                return BadRequest(response.Message);
            }
            return CreatedAtAction(nameof(GetPackage), new { id = response.Result.Id }, response.Result);
        }
        // PUT: api/packages/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePackage(int id, [FromForm] PlanDTO dto)
        {
            if (id != dto.Id)
            {
                return BadRequest();
            }
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }
            var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "Gestionar Paquetes");
            if (!hasPermission)
            {
                return Forbid();
            }
            var response = await _packageService.EditeAsync(dto);
            if (!response.IsSucess)
            {
                return BadRequest(response.Message);
            }
            return NoContent();
        }
        // DELETE: api/packages/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePackage(int id)
        {
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId))
            {
                return Unauthorized();
            }
            var hasPermission = await _permissionService.UserHasPermissionAsync(userId, "Gestionar Paquetes");
            if (!hasPermission)
            {
                return Forbid();
            }
            var response = await _packageService.DeleteAsync(id);
            if (!response.IsSucess)
            {
                return BadRequest(response.Message);
            }
            return Ok(new { message = "paquete eliminado con exito" });
        }
    }
}