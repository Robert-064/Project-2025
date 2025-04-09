using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Services;
using Project_2025_Web.DTOs;


namespace Project_2025_Web.Controllers
{
    public class PaquetesController : Controller
    {
        private readonly IPlanService _paqueteService;

        public PaquetesController(IPlanService paqueteService)
        {
            _paqueteService = paqueteService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var paquetes = await _paqueteService.GetAllPaquetesAsync();
            return View(paquetes);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var planDto = await _paqueteService.GetPaqueteByIdAsync(id);
            if (planDto == null)
            {
                return RedirectToAction("Index");
            }

            return View(planDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PlanDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _paqueteService.EditeAsync(dto);
            if (!response.IsSucess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(dto);
            }

            return RedirectToAction("Index"); // o el nombre de la vista principal de Planes
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlanDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _paqueteService.CreateAsync(dto);

            if (response.IsSucess)
            {
                return RedirectToAction("Index");
            }

            // Si hay un error en el servicio, mostrar mensaje
            ModelState.AddModelError(string.Empty, response.Message);
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _paqueteService.DeleteAsync(id);

            if (response.IsSucess)
            {
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction("Index");
        }
    }
}

