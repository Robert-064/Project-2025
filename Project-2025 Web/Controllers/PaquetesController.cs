using Microsoft.AspNetCore.Mvc;

namespace PrivateBlog.web.Controllers
{
    public class PaquetesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
