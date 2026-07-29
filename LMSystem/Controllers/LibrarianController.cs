using LMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Controllers
{
    // Same EF Core rewrite rationale as StudentController - see note there.
    [Authorize(Roles = "Administrator")]
    public class LibrarianController : Controller
    {
        private readonly LibraryContext _context;
        private const int PageSize = 5;

        public LibrarianController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            if (page < 1) page = 1;

            var query = _context.Librarians.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(l => l.Name != null && l.Name.ToLower().Contains(term));
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;

            var librarians = await query
                .OrderBy(l => l.LibrarianId)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new LibrarianIndexViewModel
            {
                Librarians = librarians,
                SearchTerm = searchTerm,
                CurrentPage = page,
                PageSize = PageSize,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LibrarianModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Librarians.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Successfully added librarian: {model.Name}.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null)
            {
                TempData["ErrorMessage"] = $"No librarian found with ID {id}.";
                return View("NotFound");
            }
            return View(librarian);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LibrarianModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _context.Librarians.FindAsync(model.LibrarianId);
            if (existing == null)
            {
                TempData["ErrorMessage"] = $"No librarian found with ID {model.LibrarianId}.";
                return View("NotFound");
            }

            existing.Name = model.Name;
            existing.Age = model.Age;
            existing.Phone = model.Phone;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Successfully updated librarian.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian != null)
            {
                _context.Librarians.Remove(librarian);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully deleted librarian.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
