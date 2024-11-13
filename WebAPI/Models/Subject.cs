using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class Subject
{
    [Key]
    public int SubjectId { get; set; }
    [Required(ErrorMessage = "Please enter subject name", AllowEmptyStrings = false)]
    public required string SubjectName { get; set; }
}