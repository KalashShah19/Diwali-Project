namespace WebAPI.Models;

public class Student
{
    public class StudentDetails
    {
        public required long EnrollmentNumber { get; set; }
        public required string Image { get; set; }
        public required string Name { get; set; }
        public required string Standard { get; set; }
        public required string Email { get; set; }
        public required string MobileNumber { get; set; }
        public Gender Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Address { get; set; }
        public DateTime AdmissionDate { get; set; }
        public bool Studying { get; set; }
    }

    public class AdmitStudent
    {
        public int UserId { get; set; }
        public required string Standard { get; set; }
        public bool Studying = true;
        public DateTime AdmissionDate = DateTime.Now;
    }

    public class AdminUpdate
    {
        public int EnrollmentNumber { get; set; }
        public required string Standard { get; set; }
        public bool Studying { get; set; }
    }
}