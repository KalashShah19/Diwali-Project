using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Please enter your email address.", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Please enter your password.", AllowEmptyStrings = false)]
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[0-9])(?=.*[A-Z])(?=.*[+!@])[a-zA-Z0-9+!@]{5,}$/", ErrorMessage = "Password should be minimum of 5 characters and must contain a capital letter, a small letter and a special symbol")]
        public required string Password { get; set; }
        // public required bool c_verified { get; set; }
    }

}