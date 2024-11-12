using Npgsql;
using WebAPI.Models;

namespace WebAPI.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly NpgsqlConnection connection;
    public SubjectRepository(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        this.connection = dataSource.OpenConnection();
    }
    public void AddSubject(string subjectname)
    {
        if (IsSubjectExist(subjectname)) throw new Exception($"Subject {subjectname} already exists");
        NpgsqlCommand addSubjectCommand = new("INSERT INTO t_subjects(c_subject_name) VALUES(@subjectname)", connection);
        addSubjectCommand.Parameters.AddWithValue("subjectname", subjectname);
        addSubjectCommand.ExecuteNonQuery();
    }

    public List<Subject> GetSubjects()
    {
        List<Subject> subjects = [];
        NpgsqlCommand getStandardsCommand = new("SELECT * from t_subjects", connection);
        using NpgsqlDataReader reader = getStandardsCommand.ExecuteReader();
        while (reader.Read())
        {
            subjects.Add(new() { SubjectId = reader.GetInt16(0), SubjectName = reader.GetString(1) });
        }
        return subjects;
    }

    private bool IsSubjectExist(string subjectname)
    {
        NpgsqlCommand getSubjectCommand = new("SELECT * FROM t_subjects WHERE LOWER(c_subject_name) LIKE LOWER(@subjectname)", connection);
          getSubjectCommand.Parameters.AddWithValue("subjectname", $"%{subjectname}%");
        using NpgsqlDataReader reader = getSubjectCommand.ExecuteReader();
        return reader.HasRows;
    }

    public void RemoveSubject(int subjectid)
    {
        NpgsqlCommand deleteStandardCommand = new("DELETE FROM t_subjects WHERE c_subject_id = @subid", connection);
        deleteStandardCommand.Parameters.AddWithValue("subid", subjectid);
        deleteStandardCommand.ExecuteNonQuery();
    }
}