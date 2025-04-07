using Microsoft.AspNetCore.Mvc;

namespace PrivateBlog.web.Controllers
{
    public class ReservasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
