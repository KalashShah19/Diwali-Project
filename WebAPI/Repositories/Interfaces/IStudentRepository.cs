using WebAPI.Models;

namespace WebAPI.Repositories;

interface IStudentRepository
{
    public void AdmitStudent(Student.AdmitStudent student);
    public List<User.AdmitRequest> GetAdmissionRequests();
    public List<Student.StudentDetails> GetStudentsList();
    public List<Student.StudentDetails> GetStudentsListByStandard(string standard);
    public void UpdateStudentDetails(Student.AdminUpdate studentDetails);
}