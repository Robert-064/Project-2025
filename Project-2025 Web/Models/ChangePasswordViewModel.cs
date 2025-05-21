using Microsoft.AspNetCore.Mvc;

namespace Project_2025_Web.Models
{
    public class ChangePasswordViewModel : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
