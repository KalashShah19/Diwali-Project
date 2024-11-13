namespace WebAPI.Models;

public class Teacher
{
    public class TeacherDetails
    {
        public required int TeacherId { get; set; }
        public required string Image { get; set; }
        public required string Name { get; set; }
        public string? Standard { get; set; }
        public required string Qualification { get; set; }
        public required string Email { get; set; }
        public required string MobileNumber { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Address { get; set; }
        public DateTime JoiningDate { get; set; }
        public bool Working { get; set; }
    }

    public class HireTeacher
    {
        public int UserId { get; set; }
        public required string Standard { get; set; }
        public required string Qualification { get; set; }
        public bool Working = true;
        public DateTime JoiningDate = DateTime.Now;
    }

    public class AdminUpdate
    {
        public int TeacherId { get; set; }
        public string? Standard { get; set; }
        public bool Working { get; set; }
    }
}