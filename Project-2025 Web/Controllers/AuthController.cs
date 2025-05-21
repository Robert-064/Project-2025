using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.ViewModels; // Usamos solo este
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Project_2025_Web.Data;
using System.Linq;
using System;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.Services;

namespace Project_2025_Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;

        public AuthController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.PasswordHash == model.Password);
            if (user != null)
            {
                HttpContext.Session.SetString("UsuarioEmail", user.Email);

                TempData["mensaje"] = "Inicio de sesión exitoso.";
                TempData["tipo"] = "success";

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Credenciales inválidas");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (!_userService.IsAuthenticated())
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var email = _userService.GetUserEmail();
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || user.PasswordHash != model.CurrentPassword)
            {
                ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta.");
                return View(model);
            }

            user.PasswordHash = model.NewPassword;
            _context.SaveChanges();

            TempData["mensaje"] = "Contraseña cambiada exitosamente.";
            TempData["tipo"] = "success";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                TempData["mensaje"] = "Si el correo está registrado, recibirás un enlace.";
                TempData["tipo"] = "info";
                return RedirectToAction("Login");
            }

            var token = Guid.NewGuid().ToString();
            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
            _context.SaveChanges();

            var resetLink = Url.Action("ResetPassword", "Auth", new { token = token }, Request.Scheme);
            System.IO.File.WriteAllText("wwwroot/reset-link.txt", $"Reset your password: {resetLink}");

            TempData["mensaje"] = "Enlace de recuperación enviado (ver reset-link.txt).";
            TempData["tipo"] = "success";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["mensaje"] = "Token inválido.";
                TempData["tipo"] = "error";
                return RedirectToAction("Login");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _context.Users.FirstOrDefault(u =>
                u.PasswordResetToken == model.Token &&
                u.ResetTokenExpires > DateTime.UtcNow);

            if (user == null)
            {
                TempData["mensaje"] = "Token inválido o expirado.";
                TempData["tipo"] = "error";
                return RedirectToAction("Login");
            }

            user.PasswordHash = model.NewPassword;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;
            _context.SaveChanges();

            TempData["mensaje"] = "Contraseña restablecida correctamente.";
            TempData["tipo"] = "success";
            return RedirectToAction("Login");
        }
    }
}
