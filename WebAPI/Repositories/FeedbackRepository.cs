using Npgsql;
using WebAPI.Models;

namespace WebAPI.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly NpgsqlConnection connection;
    public FeedbackRepository(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        this.connection = dataSource.OpenConnection();
    }

    public void AddFeedback(Feedback.Post feedback)
    {
        NpgsqlCommand addFeedbackCommand = new("INSERT INTO t_feedbacks(c_student_id, c_teacher_id, c_rating, c_comment, c_batch_year) VALUES(@studentid, @teacherid, @ratings, @comment, @batchyear)", connection);
        addFeedbackCommand.Parameters.AddWithValue("studentid", feedback.StudentID);
        addFeedbackCommand.Parameters.AddWithValue("teacherid", feedback.TeacherID);
        addFeedbackCommand.Parameters.AddWithValue("ratings", feedback.Rating);
        addFeedbackCommand.Parameters.AddWithValue("comment", !string.IsNullOrEmpty(feedback.Comment) ? feedback.Comment : DBNull.Value);
        addFeedbackCommand.Parameters.AddWithValue("batchyear", feedback.BatchYear);
        addFeedbackCommand.ExecuteNonQuery();
    }

    public List<Feedback.Get> GetFeedbacks()
    {
        // TODO("Perform Join with Teacher Table to Get Teacher Name")
        List<Feedback.Get> feedbacks = [];
        NpgsqlCommand getFeedbacksCommand = new("SELECT * from t_feedbacks", connection);
        using NpgsqlDataReader reader = getFeedbacksCommand.ExecuteReader();
        while (reader.Read())
        {
            feedbacks.Add(new() { TeacherId = reader.GetInt16(1), Rating = reader.GetInt16(2), Comment = reader.IsDBNull(3) ? null : reader.GetString(3), BatchYear = reader.GetString(4), FeedbackDate = reader.GetDateTime(5) });
        }
        return feedbacks;
    }

    public List<Feedback.TeacherGet> GetFeedbacks(int teacherid)
    {
        // TODO("Perform Join with Teacher Table to Get Teacher Name")
        List<Feedback.TeacherGet> feedbacks = [];
        NpgsqlCommand getFeedbacksCommand = new("SELECT * from t_feedbacks WHERE c_teacher_id = @teacherid", connection);
        getFeedbacksCommand.Parameters.AddWithValue("teacherid", teacherid);
        using NpgsqlDataReader reader = getFeedbacksCommand.ExecuteReader();
        while (reader.Read())
        {
            feedbacks.Add(new() { Rating = reader.GetInt16(2), Comment = reader.IsDBNull(3) ? null : reader.GetString(3), BatchYear = reader.GetString(4), FeedbackDate = reader.GetDateTime(5) });
        }
        return feedbacks;
    }

    public List<Feedback.Get> GetFeedbacksByStudent(int studentid)
    {
        List<Feedback.Get> feedbacks = [];
        NpgsqlCommand getFeedbacksCommand = new("SELECT * from t_feedbacks WHERE c_student_id = @studentid", connection);
        getFeedbacksCommand.Parameters.AddWithValue("studentid", studentid);
        using NpgsqlDataReader reader = getFeedbacksCommand.ExecuteReader();
        while (reader.Read())
        {
            feedbacks.Add(new() { TeacherId = reader.GetInt16(1), Rating = reader.GetInt16(2), Comment = reader.IsDBNull(3) ? null : reader.GetString(3), BatchYear = reader.GetString(4), FeedbackDate = reader.GetDateTime(5) });
        }
        return feedbacks;
    }
}