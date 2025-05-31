namespace Project_2025_Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Project_2025_Web.Data;
    using Project_2025_Web.Data.Entities;
    using Project_2025_Web.DTOs;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Project_2025_Web.DTO;

    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly DataContext _context;

        public RolesController(DataContext context)
        {
            _context = context;
        }

        private List<string> GetDefaultPermissionsForRole(string roleName)
        {
            switch (roleName.ToLower())
            {
                case "admin":
                    return new List<string> { "ManageUsers", "ManageRoles", "ViewReservations", "EditReservations" };

                case "user":
                    return new List<string> { "ViewPlans", "CreateReservation" };

                case "guía":
                case "guia":
                    return new List<string> { "ViewAssignedReservations", "UpdateReservationStatus" };

                default:
                    return new List<string>();
            }
        }

        // Nuevo método para obtener permisos de la BD por nombre
        private async Task<List<Permission>> GetPermissionObjectsByNamesAsync(List<string> permissionNames)
        {
            return await _context.Permissions
                .Where(p => permissionNames.Contains(p.Name))
                .ToListAsync();
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles.Include(r => r.RolePermissions)
                                            .ThenInclude(rp => rp.Permission)
                                            .ToListAsync();
            return View(roles);
        }

        public async Task<IActionResult> Create()
        {
            var dto = new RoleDTO
            {
                AvailablePermissions = await _context.Permissions.ToListAsync()
            };
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                dto.AvailablePermissions = await _context.Permissions.ToListAsync();
                return View(dto);
            }

            var role = new Role { Name = dto.Name };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            foreach (var permissionId in dto.SelectedPermissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }

            await _context.SaveChangesAsync();
            TempData["mensaje"] = "Rol creado correctamente.";
            TempData["tipo"] = "success";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles.Include(r => r.RolePermissions)
                                           .FirstOrDefaultAsync(r => r.Id == id);
            if (role == null) return NotFound();

            var model = new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                SelectedPermissionIds = role.RolePermissions.Select(rp => rp.PermissionId).ToList(),
                AvailablePermissions = await _context.Permissions.ToListAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleDTO dto)
        {
            if (id != dto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                dto.AvailablePermissions = await _context.Permissions.ToListAsync();
                return View(dto);
            }

            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return NotFound();

            role.Name = dto.Name;

            _context.RolePermissions.RemoveRange(role.RolePermissions);

            foreach (var permissionId in dto.SelectedPermissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }

            await _context.SaveChangesAsync();
            TempData["mensaje"] = "Rol actualizado correctamente.";
            TempData["tipo"] = "success";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles
                            .Include(r => r.RolePermissions)
                            .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null) return NotFound();

            // Revisar si hay usuarios con ese rol
            bool hasUsers = await _context.Users.AnyAsync(u => u.RoleId == id);

            if (hasUsers)
            {
                TempData["mensaje"] = "No se puede eliminar el rol porque tiene usuarios asignados.";
                TempData["tipo"] = "error";
                return RedirectToAction(nameof(Index));
            }

            _context.RolePermissions.RemoveRange(role.RolePermissions);
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            TempData["mensaje"] = "Rol eliminado correctamente.";
            TempData["tipo"] = "success";
            return RedirectToAction(nameof(Index));
        }

    }
}


