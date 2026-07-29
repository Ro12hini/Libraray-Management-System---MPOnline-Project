using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LMSystem.Models
{
    // Inherits from IdentityDbContext so the same MySQL database holds both the
    // library's own tables AND the ASP.NET Core Identity tables (AspNetUsers, AspNetRoles, ...).
    public class LibraryContext : IdentityDbContext<ApplicationUser>
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();
        public DbSet<Publication> Publications => Set<Publication>();
        public DbSet<StudentModel> Students => Set<StudentModel>();
        public DbSet<LibrarianModel> Librarians => Set<LibrarianModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Required: builds Identity's own tables first

            // Seed initial Book catalog
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    BookId = 1,
                    Title = "The Pragmatic Programmer",
                    Author = "Andrew Hunt and David Thomas",
                    ISBN = "978-0201616224",
                    PublishedDate = new DateTime(2021, 10, 30),
                    IsAvailable = true
                },
                new Book
                {
                    BookId = 2,
                    Title = "Design Patterns using C#",
                    Author = "Robert C. Martin",
                    ISBN = "978-0132350884",
                    PublishedDate = new DateTime(2023, 8, 1),
                    IsAvailable = true
                },
                new Book
                {
                    BookId = 3,
                    Title = "Mastering ASP.NET Core",
                    Author = "Pranaya Kumar Rout",
                    ISBN = "978-0451616235",
                    PublishedDate = new DateTime(2022, 11, 22),
                    IsAvailable = true
                },
                new Book
                {
                    BookId = 4,
                    Title = "SQL with DBA Fundamentals",
                    Author = "Rakesh Kumar",
                    ISBN = "978-4562350123",
                    PublishedDate = new DateTime(2020, 8, 15),
                    IsAvailable = true
                }
            );

            // Seed sample Newspapers / Magazines
            modelBuilder.Entity<Publication>().HasData(
                new Publication { Id = 1, Title = "The Daily Times", Publisher = "Global Media Group", PublishedDate = new DateTime(2026, 7, 22), Type = PublicationType.Newspaper, IsAvailable = true },
                new Publication { Id = 2, Title = "Financial Chronicle", Publisher = "WallSt Press", PublishedDate = new DateTime(2026, 7, 21), Type = PublicationType.Newspaper, IsAvailable = true },
                new Publication { Id = 3, Title = "Tech Weekly News", Publisher = "Silicon Valley Pubs", PublishedDate = new DateTime(2026, 7, 20), Type = PublicationType.Newspaper, IsAvailable = true },
                new Publication { Id = 4, Title = "National Geographic Vol 45", Publisher = "NatGeo Society", PublishedDate = new DateTime(2026, 7, 1), Type = PublicationType.Magazine, IsAvailable = true },
                new Publication { Id = 5, Title = "PC Gamer Ultimate", Publisher = "Future US", PublishedDate = new DateTime(2026, 7, 5), Type = PublicationType.Magazine, IsAvailable = true }
            );

            // MySQL column-type tweak: store enum as int explicitly (Pomelo default is fine, this is just explicit)
            modelBuilder.Entity<Publication>()
                .Property(p => p.Type)
                .HasConversion<int>();
        }
    }
}
