using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Project_2025_Web.Data;
using System.Linq;
using System;

namespace Project_2025_Web.Controllers
{
    public class AuthController : Controller
    {
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

            if (model.Email == "admin@demo.com" && model.Password == "123456")
            {
                HttpContext.Session.SetString("UsuarioEmail", model.Email);

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
            // await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (HttpContext.Session.GetString("UsuarioEmail") == null)
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

            var email = HttpContext.Session.GetString("UsuarioEmail");

            using (var context = new DataContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == email);

                if (user == null || user.Password != model.CurrentPassword)
                {
                    ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta.");
                    return View(model);
                }

                user.Password = model.NewPassword;
                context.SaveChanges();
            }

            TempData["mensaje"] = "Contraseña cambiada exitosamente.";
            TempData["tipo"] = "success";
            return RedirectToAction("Index", "Home");
        }

        // ---------------------------
        // RECUPERACIÓN DE CONTRASEÑA
        // ---------------------------

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

            using (var context = new DataContext())
            {
                var user = context.Users.FirstOrDefault(u => u.Email == model.Email);
                if (user == null)
                {
                    TempData["mensaje"] = "Si el correo está registrado, recibirás un enlace.";
                    TempData["tipo"] = "info";
                    return RedirectToAction("Login");
                }

                var token = Guid.NewGuid().ToString();
                user.PasswordResetToken = token;
                user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);
                context.SaveChanges();

                var resetLink = Url.Action("ResetPassword", "Auth", new { token = token }, Request.Scheme);

                // Enviar correo (simulado en archivo)
                System.IO.File.WriteAllText("wwwroot/reset-link.txt", $"Reset your password: {resetLink}");

                TempData["mensaje"] = "Enlace de recuperación enviado (ver reset-link.txt).";
                TempData["tipo"] = "success";
                return RedirectToAction("Login");
            }
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

            using (var context = new DataContext())
            {
                var user = context.Users.FirstOrDefault(u =>
                    u.PasswordResetToken == model.Token &&
                    u.ResetTokenExpires > DateTime.UtcNow);

                if (user == null)
                {
                    TempData["mensaje"] = "Token inválido o expirado.";
                    TempData["tipo"] = "error";
                    return RedirectToAction("Login");
                }

                user.Password = model.NewPassword;
                user.PasswordResetToken = null;
                user.ResetTokenExpires = null;
                context.SaveChanges();

                TempData["mensaje"] = "Contraseña restablecida correctamente.";
                TempData["tipo"] = "success";
                return RedirectToAction("Login");
            }
        }
    }
}
