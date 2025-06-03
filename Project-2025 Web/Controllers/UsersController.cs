using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using System.Security.Claims;

namespace Project_2025_Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // --- ACCIONES ADMIN ---

        // GET: Users (Listado con filtros y paginación) SOLO ADMIN
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? name, string? email, string? role, int page = 1)
        {
            int pageSize = 10;

            var rolesList = await _context.Roles
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                .ToListAsync();
            ViewBag.Roles = rolesList;

            var query = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Username.Contains(name));
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }

            if (!string.IsNullOrEmpty(role))
            {
                if (int.TryParse(role, out int roleId))
                {
                    query = query.Where(u => u.RoleId == roleId);
                }
            }

            int totalUsers = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var users = await query
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.FiltroNombre = name;
            ViewBag.FiltroCorreo = email;
            ViewBag.FiltroRol = role;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        // GET: Users/Create SOLO ADMIN
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Users/Create SOLO ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = new byte[0]; // O mejor: pedir password y hashearlo
                user.PasswordSalt = new byte[0];

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5 SOLO ADMIN
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5 SOLO ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, User user)
        {
            ModelState.Remove(nameof(user.PasswordHash));
            ModelState.Remove(nameof(user.PasswordSalt));
            ModelState.Remove(nameof(user.Role));

            if (id != user.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                    if (existingUser == null)
                        return NotFound();

                    user.PasswordHash = existingUser.PasswordHash;
                    user.PasswordSalt = existingUser.PasswordSalt;

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["mensaje"] = "¡Usuario actualizado con éxito!";
                    TempData["tipo"] = "success";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Id == user.Id))
                        return NotFound();
                    throw;
                }
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // POST: Users/Delete/5 SOLO ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // --- ACCIONES PERFIL USUARIO ---

        // GET: Users/EditProfile (Editar perfil propio, sin rol)
        public async Task<IActionResult> EditProfile()
        {
            // Obtener id usuario logueado
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Forbid();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            // No se cambia rol ni password aquí (a menos que implementes)
            return View(user);
        }

        // POST: Users/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(User user)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out int userId))
                return Forbid();

            if (user.Id != userId)
                return Forbid();

            // No permitimos cambiar password, salt ni rol en esta vista
            ModelState.Remove(nameof(user.PasswordHash));
            ModelState.Remove(nameof(user.PasswordSalt));
            ModelState.Remove(nameof(user.Role));
            ModelState.Remove(nameof(user.RoleId));

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
                    if (existingUser == null)
                        return NotFound();

                    // Mantener password y rol actuales
                    user.PasswordHash = existingUser.PasswordHash;
                    user.PasswordSalt = existingUser.PasswordSalt;
                    user.RoleId = existingUser.RoleId;

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["mensaje"] = "¡Perfil actualizado con éxito!";
                    TempData["tipo"] = "success";
                    return RedirectToAction("EditProfile");
                }
                catch (DbUpdateConcurrencyException)
                {
                    return StatusCode(500);
                }
            }
            return View(user);
        }
    }
}
