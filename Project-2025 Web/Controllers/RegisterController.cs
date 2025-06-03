using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Project_2025_Web.Data;
using Project_2025_Web.DTOs;
using Project_2025_Web.Services;
using System.Threading.Tasks;

namespace Project_2025_Web.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserService _userService;
        private readonly DataContext _context;  // Agrega esta dependencia para acceder a Roles

        public RegisterController(IUserService userService, DataContext context)
        {
            _userService = userService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserRegisterDTO dto)
        {
            // Recarga Roles siempre que se vaya a mostrar la vista
            ViewBag.Roles = new SelectList(_context.Roles, "Id", "Name");

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

