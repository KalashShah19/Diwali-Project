using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class FeesInfo
{
    public class Get
    {
        [Key]
        public int FeesID { get; set; }
        [Required(ErrorMessage = "Please enter a valid amount")]
        [DataType(DataType.Currency, ErrorMessage = "Please enter a valid amount.")]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Please enter a valid year.")]
        public required string BatchYear { get; set; }
        [Required(ErrorMessage = "Please enter a valid standard.")]
        [Range(1, 12, ErrorMessage = "Standard must be between 1 to 12")]
        public required string Standard { get; set; }
    }

    public class Post
    {
        [Required(ErrorMessage = "Please enter a valid amount")]
        [DataType(DataType.Currency, ErrorMessage = "Please enter a valid amount.")]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Please enter a valid year.")]
        public required string BatchYear { get; set; }
        [Required(ErrorMessage = "Please enter a valid standard.")]
        [Range(1, 12, ErrorMessage = "Standard must be between 1 to 12")]
        public required string Standard { get; set; }
    }
}