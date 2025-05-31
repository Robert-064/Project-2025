using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Services;
using Project_2025_Web.Models;
using Project_2025_Web.DTOs;
using Project_2025_Web.Data.Entities;
using Project_2025_Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Project_2025_Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationService _Reservationservice;
        private readonly IPlanService _planService;

        public ReservationsController(IReservationService Reservationservice, IPlanService planService)
        {
            _Reservationservice = Reservationservice;
            _planService = planService;
        }

        public async Task<IActionResult> Index(
        int page = 1,
        string status = null,
        int? userId = null,
        int? planId = null,
        string date = null)
        {
            int pageSize = 5;

            // Obtener todas las reservas
            var response = await _Reservationservice.GetListAsync();
            var allReservations = response.Result; 

            // Filtros
            if (!string.IsNullOrEmpty(status))
                allReservations = allReservations.Where(r => r.Status == status).ToList();

            if (userId.HasValue)
                allReservations = allReservations.Where(r => r.Id_User == userId.Value).ToList();

            if (planId.HasValue)
                allReservations = allReservations.Where(r => r.Id_Plan == planId.Value).ToList();

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out DateTime parsedDate))
                allReservations = allReservations.Where(r => r.Date.Date == parsedDate.Date).ToList();

            // Paginación
            int totalItems = allReservations.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            page = Math.Max(1, Math.Min(page, totalPages));
            var paginatedReservations = allReservations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ViewBag para filtros actuales
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Status = status;
            ViewBag.UserId = userId;
            ViewBag.PlanId = planId;
            ViewBag.Date = date;

             var response2 = await _planService.GetListAsync();

            if (!response2.IsSucess || response2.Result == null)
            {
                ViewBag.Planes = new List<SelectListItem>();
            }
            else
            {
                ViewBag.Planes = response2.Result.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.Name
                }).ToList();
            }

            return View(paginatedReservations);
        }



        [HttpGet]
        public async Task<IActionResult> Create(int? planId = null)
        {
            Response<List<Plan>> planesResponse = await _planService.GetListAsync();

            if (!planesResponse.IsSucess)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Planes = planesResponse.Result;

            var dto = new ReservationDTO();

            if (planId.HasValue)
            {
                dto.Id_Plan = planId.Value; // Preseleccionar el plan si viene del botón
            }

            return View(dto);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _Reservationservice.CreateAsync(dto);
            if (!response.IsSucess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(dto);
            }

            TempData["mensaje"] = "¡Tu Reservation ha sido exitosa!";
            TempData["tipo"] = "success";
            return RedirectToAction(nameof(Index));
        }

        

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ReservationResponse = await _Reservationservice.GetOneAsync(id);
            if (!ReservationResponse.IsSucess || ReservationResponse.Result == null)
            {
                return RedirectToAction("Index");
            }

            var planesResponse = await _planService.GetListAsync();
            if (!planesResponse.IsSucess || planesResponse.Result == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Planes = planesResponse.Result;

            return View(ReservationResponse.Result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _Reservationservice.EditAsync((dto));

            if (!response.IsSucess)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(dto);
            }

            TempData["mensaje"] = "¡Reservation actualizada con éxito!";
            TempData["tipo"] = "success";
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _Reservationservice.DeleteAsync(id);

            if (!response.IsSucess)
            {
                TempData["mensaje"] = "Error al eliminar la Reservation: " + response.Message;
                TempData["tipo"] = "danger";
            }
            else
            {
                TempData["mensaje"] = "¡Reservation eliminada con éxito!";
                TempData["tipo"] = "success";
            }

            return RedirectToAction(nameof(Index));
        }



    }


}


