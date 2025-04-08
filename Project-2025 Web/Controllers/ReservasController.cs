using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Services;
using Project_2025_Web.Models;
using Project_2025_Web.DTOs;
using Project_2025_Web.Data.Entities;



namespace Project_2025_Web.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IPlanService _planService;

        public ReservasController(IReservationService reservationService, IPlanService planService)
        {
            _reservationService = reservationService;
            _planService = planService;
        }

        public async Task<IActionResult> Index()
        {
            Response<List<ReservationDTO>> response = await _reservationService.GetListAsync();

            if (!response.IsSucess)
            {
                return RedirectToAction("Index");
            }
            return View(response.Result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var planesResponse = await _planService.GetListAsync();

            if (!planesResponse.IsSucess)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Planes = planesResponse.Result; //Para ver las opciones de planes


            return View(new ReservationDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Create(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _reservationService.CreateAsync(dto);

            if (!response.IsSucess)
            {
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reservationResponse = await _reservationService.GetOne(id);
            if (!reservationResponse.IsSucess || reservationResponse.Result == null)
            {
                return RedirectToAction("Index");
            }

            var planesResponse = await _planService.GetListAsync();
            if (!planesResponse.IsSucess || planesResponse.Result == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Planes = planesResponse.Result;

            return View(reservationResponse.Result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ReservationDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var response = await _reservationService.EditeAsync(dto);

            if (!response.IsSucess)
            {
                return View(dto);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _reservationService.DeleteAsync(id);

            // Aunque no hay notificaciones, puedes agregar logs si lo deseas

            return RedirectToAction(nameof(Index));
        }



    }


}


