using LMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Controllers
{
    [Authorize(Roles = "Administrator,Librarian")]
    public class DashboardController : Controller
    {
        private readonly LibraryContext _context;

        public DashboardController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var model = new DashboardModel
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalBooks = await _context.Books.CountAsync(),
                TotalLibrarians = await _context.Librarians.CountAsync(),
                TotalBorrowings = await _context.BorrowRecords.CountAsync(br => br.ReturnDate == null),
                TotalPublications = await _context.Publications.CountAsync()
            };

            return View(model);
        }
    }
}
