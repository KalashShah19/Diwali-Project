using WebAPI.Models;

namespace WebAPI.Repositories;

interface IFeedbackRepository
{
    public void AddFeedback(Feedback.Post feedback);
    public List<Feedback.Get> GetFeedbacks();
    public List<Feedback.TeacherGet> GetFeedbacks(int teacherid);
    public List<Feedback.Get> GetFeedbacksByStudent(int studentid);
}