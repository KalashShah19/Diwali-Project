using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Forgotpassword
    {
        [Required(ErrorMessage = "Please enter your email address.", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Please enter your password.", AllowEmptyStrings = false)]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{6,}$",
            ErrorMessage = "Password should be minimum of 6 characters and must contain a capital letter, a small letter and a special symbol"
        )]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please reenter your password.", AllowEmptyStrings = false)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your OTP", AllowEmptyStrings = false)]
        public required string OTP { get; set; }
    }
}
