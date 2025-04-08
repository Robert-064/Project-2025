using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.DTOs;
using Project_2025_Web.Services;

public class ReservationsController : Controller
{
    private readonly IReservationService _reservaService;
    private readonly IPlanService _paqueteService;

    public ReservationsController(IReservationService reservaService, IPlanService paqueteService)
    {
        _reservaService = reservaService;
        _paqueteService = paqueteService;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int paqueteId)
    {
        var paquete = await _paqueteService.GetPaqueteByIdAsync(paqueteId);
        if (paquete == null)
        {
            return NotFound();
        }

        var model = new ReservationDTO
        {
            Id_Plan = paqueteId,
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservationDTO model)
    {
        if (ModelState.IsValid)
        {
            var result = await _reservaService.CreateAsync(model);
            if (result.IsSucess)
            {
                return RedirectToAction("Index", "Home");
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
        var userId = int.Parse(User.FindFirst("UserId").Value); 
        var reservas = await _reservaService.GetUserReservationsAsync(userId); 
        return View(reservas);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int reservationId)
    {
        var result = await _reservaService.DeleteAsync(reservationId);
        if (result.IsSucess)
        {
            TempData["SuccessMessage"] = result.Message;
        }
        else
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(MyReservations));
    }
}

