using Microsoft.AspNetCore.Identity;

namespace LMSystem.Models
{
    // Extends the default Identity user with a couple of extra profile fields.
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
