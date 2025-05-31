using Microsoft.AspNetCore.Mvc;
using Project_2025_Web.Services;
using Project_2025_Web.DTOs;

public class PackagesController : Controller
{
    private readonly IPlanService _Packageservice;
    private readonly IPermissionService _permissionService;

    public PackagesController(IPlanService Packageservice, IPermissionService permissionService)
    {
        _Packageservice = Packageservice;
        _permissionService = permissionService;
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

        var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        bool canCreate = false;
        bool canEdit = false;
        bool canDelete = false;
        bool canCreateReservation = false;

        if (userIdStr != null)
        {
            int userId = int.Parse(userIdStr);
            canCreate = canEdit = canDelete = await _permissionService.UserHasPermissionAsync(userId, "Gestionar Paquetes");
            canCreateReservation = await _permissionService.UserHasPermissionAsync(userId, "Hacer Reservas");
        }

        ViewBag.CanCreate = canCreate;
        ViewBag.CanEdit = canEdit;
        ViewBag.CanDelete = canDelete;
        ViewBag.CanCreateReservation = canCreateReservation;

        return View(plans);
    }


    [HttpGet]
    [AuthorizePermission("Gestionar Paquetes")]
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
    [AuthorizePermission("Gestionar Paquetes")]
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

        return RedirectToAction("Index");
    }

    [HttpGet]
    [AuthorizePermission("Gestionar Paquetes")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission("Gestionar Paquetes")]
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

        ModelState.AddModelError(string.Empty, response.Message);
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizePermission("Gestionar Paquetes")]
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



