using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTOs;

namespace Project_2025_Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index(string? name, string? email, string? role, int page = 1)
        {
            int pageSize = 10;

            // Obtener roles para el filtro
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

            // Validar que page esté en rango
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Obtener usuarios de la página actual con paginación
            var users = await query
                .OrderBy(u => u.Username)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

     

            // Pasar filtros y paginación a la vista para mantener estado en la UI
            ViewBag.FiltroNombre = name;
            ViewBag.FiltroCorreo = email;
            ViewBag.FiltroRol = role;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(users);
        }

        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = new byte[0];
                user.PasswordSalt = new byte[0];

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            // Elimina validación de campos que no se editan en la vista
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

                    // Conservar password actual
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


        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
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
    }
}


