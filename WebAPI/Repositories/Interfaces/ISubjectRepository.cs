using WebAPI.Models;

namespace WebAPI.Repositories;

interface ISubjectRepository
{
    public void AddSubject(string subjectname);
    public List<Subject> GetSubjects();
    public void RemoveSubject(int subjectid);
}