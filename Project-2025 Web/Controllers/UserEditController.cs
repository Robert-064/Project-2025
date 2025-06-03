using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTO;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project_2025_Web.Controllers
{
    [Authorize]
    public class UserEditController : Controller
    {
        private readonly DataContext _context;

        public UserEditController(DataContext context)
        {
            _context = context;
        }

        // GET: UserEdit/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null || !int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var model = new UserEditProfileDTO
            {
                Email = user.Email,
                Username = user.Username,
                RoleName = user.Role?.Name
            };

            return View(model);
        }

        // POST: UserEdit/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UserEditProfileDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Unauthorized();

            if (!ModelState.IsValid)
                return View("EditUserData", dto);

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
                !VerifyPassword(dto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                ModelState.AddModelError(nameof(dto.CurrentPassword), "La contraseña actual es incorrecta.");
                return View("EditUserData", dto);
            }

            user.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                CreatePasswordHash(dto.NewPassword, out byte[] hash, out byte[] salt);
                user.PasswordHash = hash;
                user.PasswordSalt = salt;
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            dto.CurrentPassword = dto.NewPassword = dto.ConfirmNewPassword = string.Empty;
            dto.RoleName = user.Role?.Name ?? "";

            TempData["mensaje"] = "Perfil actualizado correctamente.";
            TempData["tipo"] = "success";

            return View("EditUserData", dto);
        }

        // Este método ya no se usa si haces logout directo, pero lo dejamos por si quieres revertirlo luego
        [HttpPost]
        public async Task<IActionResult> VerifyPassword([FromBody] PasswordCheckModel model)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
                return Json(new { success = false, message = "Sesión no válida." });

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return Json(new { success = false, message = "Usuario no encontrado." });

            bool valid = VerifyPassword(model.Password, user.PasswordHash, user.PasswordSalt);
            return Json(new { success = valid, message = valid ? "OK" : "Contraseña incorrecta." });
        }

        // ✅ NUEVO: Acción para cerrar sesión
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Login"); 
        }

        private bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public class PasswordCheckModel
        {
            public string Password { get; set; } = null!;
        }
    }
}
