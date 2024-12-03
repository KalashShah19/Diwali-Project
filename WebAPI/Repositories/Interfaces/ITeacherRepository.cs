using WebAPI.Models;

namespace WebAPI.Repositories;

interface ITeacherRepository
{
    public List<User.AdmitRequest> GetJobRequests();
    public void HireTeacher(Teacher.HireTeacher teacher);
    public List<Teacher.TeacherDetails> GetTeacherList();
    public void UpdateTeacherDetails(Teacher.AdminUpdate teacherDetails);
}