using System.ComponentModel.DataAnnotations;

namespace mvc_task8.Models
{
    public class RegisterDTO
    {
        [Required]
        public string FullName { get; set; } = "";


        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";

    }
}

