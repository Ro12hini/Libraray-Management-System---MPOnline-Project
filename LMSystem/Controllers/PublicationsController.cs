using LMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Controllers
{
    // Handles both the Newspaper and Magazine modules, distinguished by the
    // "type" route parameter (e.g. /Publications/Index/Newspaper).
    public class PublicationsController : Controller
    {
        private readonly LibraryContext _context;
        private const int PageSize = 5;

        public PublicationsController(LibraryContext context)
        {
            _context = context;
        }

        // GET: Publications/Index/Newspaper or Publications/Index/Magazine
        [AllowAnonymous]
        public async Task<IActionResult> Index(string type, string? searchString, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(type)) return BadRequest();
            if (!Enum.TryParse(type, true, out PublicationType pubType)) return NotFound();

            ViewData["CurrentType"] = type;
            ViewData["CurrentFilter"] = searchString;

            var items = _context.Publications.AsNoTracking().Where(p => p.Type == pubType).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(p =>
                    (p.Title != null && p.Title.Contains(searchString)) ||
                    (p.Publisher != null && p.Publisher.Contains(searchString)));
            }

            var totalItems = await items.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages;

            var paginatedList = await items
                .OrderBy(p => p.Id)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewData["PageNumber"] = pageNumber;
            ViewData["TotalPages"] = totalPages;

            return View(paginatedList);
        }

        // GET: Publications/Create
        [Authorize(Roles = "Administrator,Librarian")]
        public IActionResult Create(string type)
        {
            ViewData["CurrentType"] = type;
            return View();
        }

        // POST: Publications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> Create([Bind("Title,Publisher,PublishedDate,Type")] Publication publication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { type = publication.Type.ToString() });
            }
            ViewData["CurrentType"] = publication.Type.ToString();
            return View(publication);
        }

        // GET: Publications/Edit/5
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var publication = await _context.Publications.FindAsync(id);
            if (publication == null) return NotFound();

            ViewData["CurrentType"] = publication.Type.ToString();
            return View(publication);
        }

        // POST: Publications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator,Librarian")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Publisher,PublishedDate,Type,IsAvailable")] Publication publication)
        {
            if (id != publication.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(publication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { type = publication.Type.ToString() });
            }
            ViewData["CurrentType"] = publication.Type.ToString();
            return View(publication);
        }

        // GET: Publications/Delete/5
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var publication = await _context.Publications.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (publication == null) return NotFound();
            return View(publication);
        }

        // POST: Publications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var publication = await _context.Publications.FindAsync(id);
            if (publication != null)
            {
                _context.Publications.Remove(publication);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index), new { type = publication?.Type.ToString() ?? "Newspaper" });
        }
    }
}
