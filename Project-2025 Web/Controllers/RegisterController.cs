using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.DTOs;
using Project_2025_Web.Services;
using System.Threading.Tasks;

namespace Project_2025_Web.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserService _userService;

        public RegisterController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UserRegisterDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _userService.RegisterUserAsync(dto);

            if (!result.IsSucess)
            {
                foreach (var error in result.Errors ?? new List<string>())
                {
                    ModelState.AddModelError(string.Empty, error);
                }

                if (!string.IsNullOrEmpty(result.Message))
                {
                    ModelState.AddModelError(string.Empty, result.Message);
                }

                return View(dto);
            }

            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction("Index", "Login");
        }
    }
}



