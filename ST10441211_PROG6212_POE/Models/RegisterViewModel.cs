using System.ComponentModel.DataAnnotations;

namespace ST10441211_PROG6212_POE.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public Role Role { get; set; } = Role.Lecturer;
    }
}
