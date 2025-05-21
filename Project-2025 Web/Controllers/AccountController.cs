
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Models;
using Project_2025_Web.ViewModels;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Inicio de sesión inválido.");
        }
        return View(model);
    }

    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }
}
[HttpGet]
public IActionResult ChangePassword() => View();

[HttpPost]
public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    var user = await _userManager.GetUserAsync(User);
    if (user == null) return RedirectToAction("Login");

    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

    if (result.Succeeded)
    {
        await _signInManager.RefreshSignInAsync(user);
        ViewBag.Message = "Contraseña actualizada correctamente.";
        return View();
    }

    foreach (var error in result.Errors)
        ModelState.AddModelError("", error.Description);

    return View(model);
}
@model ChangePasswordViewModel

<h2>Cambiar Contraseña</h2>

<form asp-action= "ChangePassword" method= "post" >
    < div class= "form-group" >
        < label asp -for= "CurrentPassword" ></ label >
        < input asp -for= "CurrentPassword" class= "form-control" />
        < span asp - validation -for= "CurrentPassword" class= "text-danger" ></ span >
    </ div >
    < div class= "form-group" >
        < label asp -for= "NewPassword" ></ label >
        < input asp -for= "NewPassword" class= "form-control" />
        < span asp - validation -for= "NewPassword" class= "text-danger" ></ span >
    </ div >
    < div class= "form-group" >
        < label asp -for= "ConfirmPassword" ></ label >
        < input asp -for= "ConfirmPassword" class= "form-control" />
        < span asp - validation -for= "ConfirmPassword" class= "text-danger" ></ span >
    </ div >
    < button type = "submit" class= "btn btn-success" > Actualizar </ button >
</ form >

@if(ViewBag.Message != null)
{
    < div class= "alert alert-success" > @ViewBag.Message </ div >
}

    [HttpGet]
public async Task<IActionResult> UpdateProfile()
{
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
        return RedirectToAction("Login");

    var model = new UpdateProfileViewModel
    {
        Email = user.Email,
        FullName = user.UserName // O usa un campo personalizado si lo tienes
    };

    return View(model);
}

[HttpPost]
public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var user = await _userManager.GetUserAsync(User);
    if (user == null)
        return RedirectToAction("Login");

    user.Email = model.Email;
    user.UserName = model.FullName; // Si usas otro campo para el nombre, cámbialo aquí

    var result = await _userManager.UpdateAsync(user);
    if (result.Succeeded)
    {
        ViewBag.Message = "Perfil actualizado correctamente";
        return View(model);
    }

    foreach (var error in result.Errors)
        ModelState.AddModelError("", error.Description);

    return View(model);
}
