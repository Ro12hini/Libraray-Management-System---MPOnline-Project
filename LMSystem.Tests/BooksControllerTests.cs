using FluentAssertions;
using LMSystem.Controllers;
using LMSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Tests
{
    // Uses the EF Core InMemory provider so these tests run without needing a real
    // MySQL server. Each test gets a uniquely-named in-memory database.
    public class BooksControllerTests : IDisposable
    {
        private readonly LibraryContext _context;
        private readonly BooksController _controller;

        public BooksControllerTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new LibraryContext(options);
            _context.Database.EnsureCreated();
            SeedDatabase();

            _controller = new BooksController(_context);
        }

        private void SeedDatabase()
        {
            _context.Books.AddRange(new List<Book>
            {
                new Book { BookId = 1, Title = "Bootstrap Fundamentals", Author = "Amir", ISBN = "888-0201616224", PublishedDate = DateTime.Parse("2026-07-24"), IsAvailable = true },
                new Book { BookId = 2, Title = "Node.js in Action", Author = "Shadab", ISBN = "888-0201616225", PublishedDate = DateTime.Parse("2026-07-18"), IsAvailable = true },
                new Book { BookId = 3, Title = "Software Engineering", Author = "Raju", ISBN = "888-0201616226", PublishedDate = DateTime.Parse("2026-07-24"), IsAvailable = true }
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task Index_FiltersBooks_WhenSearchQueryIsProvided()
        {
            var result = await _controller.Index(searchQuery: "node", page: 1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<BookListViewModel>().Subject;

            model.Books.Should().ContainSingle();
            model.Books.First().Title.Should().Be("Node.js in Action");
        }

        [Fact]
        public async Task Index_ReturnsAllSeededBooks_WhenNoSearchQuery()
        {
            var result = await _controller.Index(searchQuery: null, page: 1);

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<BookListViewModel>().Subject;

            model.Books.Should().HaveCount(3);
            model.TotalPages.Should().Be(1); // page size 5, only 3 seeded rows
        }

        [Fact]
        public async Task Create_AddsBook_WhenModelStateIsValid()
        {
            var newBook = new Book
            {
                Title = "Clean Architecture",
                Author = "Robert C. Martin",
                ISBN = "978-0134494166",
                PublishedDate = DateTime.Parse("2017-09-20")
            };

            var result = await _controller.Create(newBook);

            result.Should().BeOfType<RedirectToActionResult>()
                .Which.ActionName.Should().Be(nameof(BooksController.Index));

            (await _context.Books.CountAsync()).Should().Be(4);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
