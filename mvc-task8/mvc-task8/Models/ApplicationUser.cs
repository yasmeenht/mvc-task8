using Microsoft.AspNetCore.Identity;

namespace mvc_task8.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
    }
}

