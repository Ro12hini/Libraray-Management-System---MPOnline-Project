using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Error() => View();
    }
}
