using System.ComponentModel.DataAnnotations;

namespace LMSystem.Models
{
    public class StudentModel
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(100)]
        [Display(Name = "Student Name")]
        public string? StudentName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        public string? Phone { get; set; }
    }
}
