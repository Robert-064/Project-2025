﻿using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.DTOs;
using Project_2025_Web.Services;
using System.Threading.Tasks;
namespace Project_2025_Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IUserService _userService;
        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }
        // POST: api/Register
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
    var result = await _userService.RegisterApiUserAsync(registerDto);
            if (!result.IsSucess)
            {
                if (result.Errors != null && result.Errors.Count > 0)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
                else if (!string.IsNullOrEmpty(result.Message))
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }
                return BadRequest(ModelState);
            }
            return Ok(new
            {
                message = result.Message,
        token = result.Result.Token,
        expiration = result.Result.Expiration
            });
        }
    }
}
