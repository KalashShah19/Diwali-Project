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
    public class FeesDetailsForStudent
    {
        public int? c_paymentid { get; set; }
        public int c_enrollment_number { get; set; }
        public string c_currentstandard { get; set; }
        public int c_amount { get; set; }
        public string c_status { get; set; }
        public DateTime? c_paymentdate { get; set; }
        public int? c_id { get; set; } 
    }

    public class PaymentRequest
    {
        public int UserId { get; set; }
        public string CurrentStandard { get; set; }
        public int FeeStructureId { get; set; }
        public string Status { get; set; } 
        public DateTime PaymentDate { get; set; } 
    }

}