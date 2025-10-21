using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace ST10441211_PROG6212_POE.Models
{
    public class LoginModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
