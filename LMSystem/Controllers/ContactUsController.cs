using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Controllers
{
    public class ContactUsController : Controller
    {
        public IActionResult Index() => View();

        // Placeholder POST handler - wire up an email/notification service here.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string name, string email, string message)
        {
            TempData["SuccessMessage"] = "Thanks for reaching out - our support team will respond within 24 hours.";
            return RedirectToAction(nameof(Index));
        }
    }
}
