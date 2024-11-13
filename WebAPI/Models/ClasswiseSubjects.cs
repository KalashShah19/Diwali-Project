using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models;

public class ClasswiseSubjects
{
    public class Default
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public required string Standard { get; set; }
    }

    public class Get
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public required string TeacherName { get; set; }
        public int SubjectId { get; set; }
        public required string SubjectName { get; set; }
        public required string Standard { get; set; }
    }

    public class Post
    {
        [Required(ErrorMessage = "Please select the subject teacher.")]
        public int TeacherId { get; set; }
        [Required(ErrorMessage = "Please select the subject.")]
        public int SubjectId { get; set; }
        [Required(ErrorMessage = "Please select the standard.")]
        public required string Standard { get; set; }
    }
}