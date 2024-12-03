using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace MVC.Models
{
    public class Forgotpassword
    {
        [Required(ErrorMessage = "Please enter your password.", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d).{6,}$", ErrorMessage = "Password should be minimum of 6 characters and must contain a capital letter, a small letter and a special symbol")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Please reenter your password.", AllowEmptyStrings = false)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter your OTP", AllowEmptyStrings = false)]
        public required string OTP { get; set; }
    }
}