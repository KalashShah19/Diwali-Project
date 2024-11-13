using Npgsql;
using WebAPI.Libraries;
using WebAPI.Models;

namespace WebAPI.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly NpgsqlConnection connection;
    public StudentRepository(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        this.connection = dataSource.OpenConnection();
    }

    public void AdmitStudent(Student.AdmitStudent student)
    {
        NpgsqlCommand verifyStudentCommand = new("UPDATE t_users SET c_verified = true WHERE c_user_id = @userid", connection);
        verifyStudentCommand.Parameters.AddWithValue("userid", student.UserId);
        verifyStudentCommand.ExecuteNonQuery();

        NpgsqlCommand admitStudentCommand = new("INSERT INTO t_students(c_user_id, c_standard, c_admission_date, c_studying) VALUES(@userid, @standard, @admissiondate, @studying)", connection);
        admitStudentCommand.Parameters.AddWithValue("userid", student.UserId);
        admitStudentCommand.Parameters.AddWithValue("standard", student.Standard);
        admitStudentCommand.Parameters.AddWithValue("admissiondate", student.AdmissionDate);
        admitStudentCommand.Parameters.AddWithValue("studying", student.Studying);
        admitStudentCommand.ExecuteNonQuery();
    }

    public List<User.AdmitRequest> GetAdmissionRequests()
    {
        List<User.AdmitRequest> admissionRequests = [];
        NpgsqlCommand getAdmissionRequestCommand = new("SELECT * FROM t_users where c_role = 'Student' AND c_verified = false", connection);
        using NpgsqlDataReader reader = getAdmissionRequestCommand.ExecuteReader();
        while (reader.Read())
        {
            admissionRequests.Add(new() { UserId = reader.GetInt16(0), Image = reader.GetString(1), Name = reader.GetString(2), Email = reader.GetString(3), MobileNumber = reader.GetString(5), Gender = EnumHelper.GetGender(reader.GetString(6)), BirthDate = reader.GetDateTime(7), Address = reader.GetString(8) });
        }
        return admissionRequests;
    }

    public List<Student.StudentDetails> GetStudentsList()
    {
        List<Student.StudentDetails> studentDetails = [];
        NpgsqlCommand getAdmissionRequestCommand = new("SELECT c_enrollment_number, c_image, c_name, c_standard, c_email, c_mobile_number, c_gender, c_birth_date, c_address, c_admission_date, c_studying FROM t_students inner join t_users ON t_students.c_user_id = t_users.c_user_id", connection);
        using NpgsqlDataReader reader = getAdmissionRequestCommand.ExecuteReader();
        while (reader.Read())
        {
            studentDetails.Add(new() { EnrollmentNumber = reader.GetInt64(0), Image = reader.GetString(1), Name = reader.GetString(2), Standard = reader.GetString(3), Email = reader.GetString(4), MobileNumber = reader.GetString(5), Gender = EnumHelper.GetGender(reader.GetString(6)), BirthDate = reader.GetDateTime(7), Address = reader.GetString(8), AdmissionDate = reader.GetDateTime(9), Studying = reader.GetBoolean(10) });
        }
        return studentDetails;
    }

    public List<Student.StudentDetails> GetStudentsListByStandard(string standard)
    {
        List<Student.StudentDetails> studentDetails = [];
        NpgsqlCommand getAdmissionRequestCommand = new("SELECT c_enrollment_number, c_image, c_name, c_standard, c_email, c_mobile_number, c_gender, c_birth_date, c_address, c_admission_date, c_studying FROM t_students inner join t_users ON t_students.c_user_id = t_users.c_user_id WHERE c_standard = @standard", connection);
        getAdmissionRequestCommand.Parameters.AddWithValue("standard", standard);
        using NpgsqlDataReader reader = getAdmissionRequestCommand.ExecuteReader();
        while (reader.Read())
        {
            studentDetails.Add(new() { EnrollmentNumber = reader.GetInt64(0), Image = reader.GetString(1), Name = reader.GetString(2), Standard = reader.GetString(3), Email = reader.GetString(4), MobileNumber = reader.GetString(5), Gender = EnumHelper.GetGender(reader.GetString(6)), BirthDate = reader.GetDateTime(7), Address = reader.GetString(8), AdmissionDate = reader.GetDateTime(9), Studying = reader.GetBoolean(10) });
        }
        return studentDetails;
    }

    public void UpdateStudentDetails(Student.AdminUpdate studentDetails)
    {
        NpgsqlCommand updateStudentCommand = new("UPDATE t_students SET c_standard = @standard, c_studying = @studying WHERE c_enrollment_number = @enroll", connection);
        updateStudentCommand.Parameters.AddWithValue("standard", studentDetails.Standard);
        updateStudentCommand.Parameters.AddWithValue("studying", studentDetails.Studying);
        updateStudentCommand.Parameters.AddWithValue("enroll", studentDetails.EnrollmentNumber);
        updateStudentCommand.ExecuteNonQuery();
    }
}