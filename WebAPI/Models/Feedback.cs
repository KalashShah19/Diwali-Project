using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class Feedback
{
    public class TeacherGet
    {
        public required int Rating { get; set; }
        public string? Comment { get; set; }
        public required string BatchYear { get; set; }
        public required DateTime FeedbackDate { get; set; }
    }

    public class Get
    {
        public required int TeacherId { get; set; }
        public required int Rating { get; set; }
        public string? Comment { get; set; }
        public required string BatchYear { get; set; }
        public required DateTime FeedbackDate { get; set; }
    }

    public class Post
    {
        [Required(ErrorMessage = "Please enter student id.")]
        public required int StudentID { get; set; }
        [Required(ErrorMessage = "Please enter teacher id.")]
        public required int TeacherID { get; set; }
        [Required(ErrorMessage = "Please enter your ratings.")]
        public required int Rating { get; set; }
        public string? Comment { get; set; }
        [Required(ErrorMessage = "Please enter batch year.")]
        public required string BatchYear { get; set; }
    }
}