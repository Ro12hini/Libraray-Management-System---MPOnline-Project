using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace LMSystem.Models
{
    // Represents a physical book in the library catalog.
    public class Book
    {
        [BindNever]
        public int BookId { get; set; } // Primary key - never bound from user input (over-posting protection)

        [Required(ErrorMessage = "The Title field is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "The Author field is required.")]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
        public string? Author { get; set; }

        [Required(ErrorMessage = "The ISBN field is required.")]
        [RegularExpression(@"^\d{3}-\d{10}$", ErrorMessage = "ISBN must be in the format XXX-XXXXXXXXXX.")]
        public string? ISBN { get; set; }

        [Required(ErrorMessage = "The Published Date field is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Published Date")]
        public DateTime PublishedDate { get; set; }

        [BindNever]
        [Display(Name = "Available")]
        public bool IsAvailable { get; set; } = true; // Controlled only by server-side logic

        // Navigation Property
        [BindNever]
        public ICollection<BorrowRecord>? BorrowRecords { get; set; }
    }
}
