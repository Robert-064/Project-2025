using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project_2025_Web.Data;
using Project_2025_Web.Data.Entities;

namespace Project_2025_Web.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly IPackageService _packageService;

        public ReservationsController(IReservationService reservationService, IPackageService packageService)
        {
            _reservationService = reservationService;
            _packageService = packageService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int packageId)
        {
            var package = await _packageService.GetPackageByIdAsync(packageId);
            if (package == null)
            {
                return NotFound();
            }

            var reservationForm = new ReservationFormViewModel
            {
                PackageId = packageId,
                PackageName = package.Name,
                MinPeople = package.MinPeople,
                MaxPeople = package.MaxPeople
            };

            return View(reservationForm);
        }


        [HttpPost]
        public async Task<IActionResult> Create(ReservationFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _reservationService.CreateReservationAsync(model);
                if (result.IsSuccess)
                {
                    return RedirectToAction("Index", "Packages");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> MyReservations()
        {
            var reservations = await _reservationService.GetUserReservationsAsync(User.Identity.Name);
            return View(reservations);
        }


        [HttpPost]
        public async Task<IActionResult> Cancel(int reservationId)
        {
            var result = await _reservationService.CancelReservationAsync(reservationId);
            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Reserva cancelada exitosamente.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(MyReservations));
        }
    }
}
