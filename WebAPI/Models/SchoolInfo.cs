using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class SchoolInfo
{
    public class Get
    {
        public required string SchoolAddress { get; set; }
        public required string SchoolContactNumber { get; set; }
        public required string PrincipalImage { get; set; }
        public required string PrincipalName { get; set; }
        public required string PrincipalQualification { get; set; }
    }

    public class Post
    {
        [Required(ErrorMessage = "Please enter school address.", AllowEmptyStrings = false)]
        public required string SchoolAddress { get; set; }
        [Required(ErrorMessage = "Please enter school contact number.", AllowEmptyStrings = false)]
        [RegularExpression(@"^[6789][\d]{9}$", ErrorMessage = "Please enter a valid mobile number.")]
        public required string SchoolContactNumber { get; set; }
        public IFormFile? PrincipalImage { get; set; }
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Please enter a valid name.")]
        [Required(ErrorMessage = "Please enter principal name.")]
        public required string PrincipalName { get; set; }
        [RegularExpression(@"^[a-zA-Z.\s]+$", ErrorMessage = "Please enter a valid qualification.")]
        [Required(ErrorMessage = "Please enter principal qualification.")]
        public required string PrincipalQualification { get; set; }
    }
}