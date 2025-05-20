using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Services;
using Project_2025_Web.DTOs;


namespace Project_2025_Web.Controllers
{
    public class PackagesController : Controller
    {
        private readonly IPlanService _Packageservice;

        public PackagesController(IPlanService Packageservice)
        {
            _Packageservice = Packageservice;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 2;

            var plans = await _Packageservice.GetPlansPagedAsync(page, pageSize);

            int totalPlans = await _Packageservice.GetPlansCountAsync();
            int totalPages = (int)Math.Ceiling((double)totalPlans / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(plans);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var planDto = await _Packageservice.GetPackageByIdAsync(id);
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

            var response = await _Packageservice.EditeAsync(dto);
            if (!response.IsSucess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(dto);
            }
            TempData["mensaje"] = "¡Plan editado con éxito!";
            TempData["tipo"] = "success";

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

            var response = await _Packageservice.CreateAsync(dto);

            if (response.IsSucess)
            {
                TempData["mensaje"] = "¡Plan creado con éxito!";
                TempData["tipo"] = "success";
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
            var response = await _Packageservice.DeleteAsync(id);

            if (response.IsSucess)
            {
                TempData["mensaje"] = "¡Eliminado con éxito!";
                TempData["tipo"] = "success";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = response.Message;
            TempData["tipo"] = "error";
            return RedirectToAction("Index");
        }




    }
}


