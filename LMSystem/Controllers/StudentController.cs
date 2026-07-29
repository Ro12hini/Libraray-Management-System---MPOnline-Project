using LMSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Controllers
{
    // NOTE: The original course material used raw ADO.NET (SqlConnection/SqlCommand)
    // with SQL Server's OFFSET/FETCH syntax. That is rewritten here using EF Core
    // LINQ (Skip/Take) so the exact same code works unmodified against MySQL.
    [Authorize(Roles = "Administrator,Librarian")]
    public class StudentController : Controller
    {
        private readonly LibraryContext _context;
        private const int PageSize = 5;

        public StudentController(LibraryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchTerm, int page = 1)
        {
            var viewModel = new StudentIndexViewModel
            {
                SearchTerm = searchTerm,
                CurrentPage = page < 1 ? 1 : page,
                PageSize = PageSize
            };

            var query = _context.Students.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(s =>
                    (s.StudentName != null && s.StudentName.ToLower().Contains(term)) ||
                    (s.Email != null && s.Email.ToLower().Contains(term)) ||
                    (s.Phone != null && s.Phone.ToLower().Contains(term)));
            }

            int totalRecords = await query.CountAsync();
            viewModel.TotalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);

            if (viewModel.CurrentPage > viewModel.TotalPages && viewModel.TotalPages > 0)
            {
                viewModel.CurrentPage = viewModel.TotalPages;
            }

            viewModel.Students = await query
                .OrderBy(s => s.StudentId)
                .Skip((viewModel.CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return View(viewModel);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _context.Students.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Successfully added student: {model.StudentName}.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                TempData["ErrorMessage"] = $"No student found with ID {id}.";
                return View("NotFound");
            }
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existing = await _context.Students.FindAsync(model.StudentId);
            if (existing == null)
            {
                TempData["ErrorMessage"] = $"No student found with ID {model.StudentId}.";
                return View("NotFound");
            }

            existing.StudentName = model.StudentName;
            existing.Email = model.Email;
            existing.Phone = model.Phone;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Successfully updated student.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Successfully deleted student.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
