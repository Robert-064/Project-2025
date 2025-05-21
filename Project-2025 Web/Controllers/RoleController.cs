using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.DTO;
using Project_2025_Web.Services;

namespace Project_2025_Web.Controllers
{
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _roleService.GetListAsync();
            return View(response.Result ?? new List<RoleDTO>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var response = await _roleService.CreateAsync(dto);
            if (!response.IsSucess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _roleService.GetOneAsync(id);
            if (!response.IsSucess || response.Result == null)
                return RedirectToAction(nameof(Index));
            return View(response.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleDTO dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var response = await _roleService.EditAsync(dto);
            if (!response.IsSucess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(dto);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _roleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
