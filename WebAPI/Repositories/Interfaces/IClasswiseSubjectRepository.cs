using WebAPI.Models;

namespace WebAPI.Repositories;

interface IClasswiseSubjectRepository
{
    public void AddClasswiseSubject(ClasswiseSubjects.Post addDetails);
    public List<ClasswiseSubjects.Get> GetClasswiseSubjects();
    public void UpdateClasswiseSubject(ClasswiseSubjects.Default details);
    public void RemoveClasswiseSubject(int id);
}