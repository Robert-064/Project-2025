using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Services;
using Project_2025_Web.DTOs;

namespace Project_2025_Web.Controllers
{
    public class PaquetesController : Controller
    {
        private readonly IPaqueteService _paqueteService;

        public PaquetesController(IPaqueteService paqueteService)
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
        public async Task<IActionResult> Details(int id)
        {
            var paquete = await _paqueteService.GetPaqueteByIdAsync(id);
            if (paquete == null)
            {
                return NotFound();
            }

            return View(paquete);
        }
    }
}

