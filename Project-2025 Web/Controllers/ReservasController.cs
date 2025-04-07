public class ReservationsController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly IPaqueteService _paqueteService;

    public ReservationsController(IReservaService reservaService, IPaqueteService paqueteService)
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

        var model = new ReservaDTO
        {
            PaqueteId = paqueteId,
            PaqueteName = paquete.Name,
            Difficulty = paquete.Difficulty,
            Distance = paquete.Distance,
            MinPeople = paquete.MinPeople,
            MaxPeople = paquete.MaxPeople
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservaDTO model)
    {
        if (ModelState.IsValid)
        {
            var result = await _reservaService.CreateReservationAsync(model);
            if (result.IsSuccess)
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
        var userName = User.Identity.Name;  
        var reservas = await _reservaService.GetUserReservationsAsync(userName);
        return View(reservas);
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int reservationId)
    {
        var result = await _reservaService.CancelReservationAsync(reservationId);
        if (result.IsSuccess)
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

