using Npgsql;
using WebAPI.Models;
using Newtonsoft.Json;

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
        List<Feedback.Get> feedbacks = [];
        NpgsqlCommand getFeedbacksCommand = new("SELECT c_feedback_date, c_name, c_rating, c_comment, c_batch_year from t_feedbacks tf INNER JOIN t_teachers tt ON tf.c_teacher_id = tt.c_teacher_id INNER JOIN t_users tu ON tu.c_user_id = tt.c_user_id", connection);
        using NpgsqlDataReader reader = getFeedbacksCommand.ExecuteReader();
        while (reader.Read())
        {
            feedbacks.Add(new() { TeacherName = reader.GetString(1), Rating = reader.GetInt16(2), Comment = reader.IsDBNull(3) ? null : reader.GetString(3), BatchYear = reader.GetString(4), FeedbackDate = reader.GetDateTime(0) });
        }
        return feedbacks;
    }

    public List<Feedback.TeacherGet> GetFeedbacks(int teacherid)
    {
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

    public List<Feedback.GetStudentFeedback> GetFeedbacksByStudent(int studentid)
    {
        List<Feedback.GetStudentFeedback> feedbacks = [];

        NpgsqlCommand getFeedbacksCommand = new("SELECT tu.c_name AS teacher_name,tfs.c_comment AS feedback_comment,tfs.c_rating AS feedback_rating,tfs.c_batch_year AS feedback_batch_year,tfs.c_feedback_date AS feedback_date FROM t_feedbacks AS tfs INNER JOIN t_teachers AS tt ON tfs.c_teacher_id = tt.c_teacher_id INNER JOIN t_users AS tu ON tt.c_user_id = tu.c_user_id WHERE tfs.c_student_id = @studentid AND tu.c_role ILIKE 'Teacher';", connection);
        getFeedbacksCommand.Parameters.AddWithValue("studentid", studentid);
        using NpgsqlDataReader reader = getFeedbacksCommand.ExecuteReader();
        while (reader.Read())
        {
            feedbacks.Add(new() { TeacherName = reader.GetString(0), Comment = reader.IsDBNull(1) ? null : reader.GetString(1), Rating = reader.GetInt16(2), BatchYear = reader.GetString(3), FeedbackDate = reader.GetDateTime(4) });
        }
        return feedbacks;
    }

    public string GetFeedbackCount(int teacherid)
    {
        NpgsqlCommand cmd = new NpgsqlCommand("SELECT avg(c_rating),count(*)  FROM public.t_feedbacks where c_teacher_id = @id", connection);
        cmd.Parameters.AddWithValue("id", teacherid);
        using NpgsqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {

            var response = new
            {
                avgrating = (double)reader.GetDouble(0),
                count = (int)reader.GetInt32(1)
            };

            return JsonConvert.SerializeObject(response);
        }
        return "";
    }


    public List<Feedback.Teacher> GetTeachers()
    {
        var teachers = new List<Feedback.Teacher>();

        string query = "SELECT t.c_teacher_id, u.c_name FROM t_users u INNER JOIN t_teachers t ON u.c_user_id = t.c_user_id WHERE u.c_role = 'Teacher'";
        using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
        {
            using (NpgsqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    teachers.Add(new Feedback.Teacher
                    {
                        c_user_id = int.Parse(reader["c_teacher_id"].ToString()),
                        c_name = reader["c_name"].ToString()
                    });
                }
            }
        }

        return teachers;
    }
}