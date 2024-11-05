using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class User
{
    public class Register
    {
        [Required(ErrorMessage = "Please select your image.")]
        public required IFormFile Image { get; set; }
        [Required(ErrorMessage = "Please enter your name.", AllowEmptyStrings = false)]
        public required string Name { get; set; }
        [Required(ErrorMessage = "Please enter your email address.", AllowEmptyStrings = false)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public required string Email { get; set; }
        [Required(ErrorMessage = "Please enter your password.", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[0-9])(?=.*[A-Z])(?=.*[+!@])[a-zA-Z0-9+!@]{8,}$/", ErrorMessage = "Password should be minimum of 8 characters and must contain a capital letter, a small letter and a special symbol")]
        public required string Password { get; set; }
        [Required(ErrorMessage = "Please reenter your password.", AllowEmptyStrings = false)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Please enter your mobile number.", AllowEmptyStrings = false)]
        [RegularExpression(@"^[6789][\d]{9}$", ErrorMessage = "Please enter a valid mobile number.")]
        public required string MobileNumber { get; set; }
        [Required(ErrorMessage = "Please select your gender.")]
        public Gender Gender { get; set; }
        [Required(ErrorMessage = "Please enter your date of birth.")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "Please enter your address.", AllowEmptyStrings = false)]
        public required string Address { get; set; }
        public bool Verified { get; set; } = false;
        public UserRole Role { get; set; }
    }

    public class AdmissionRequest
    {
        public int UserId { get; set; }
        public required string Image { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string MobileNumber { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Address { get; set; }
    }
}