using Npgsql;
using WebAPI.Libraries;
using WebAPI.Models;

namespace WebAPI.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly NpgsqlConnection connection;
    public TeacherRepository(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
        if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        this.connection = dataSource.OpenConnection();
    }

    public List<User.AdmitRequest> GetJobRequests()
    {
        List<User.AdmitRequest> jobRequests = [];
        NpgsqlCommand getJobRequestCommand = new("SELECT * FROM t_users where c_role = 'Teacher' AND c_verified = false", connection);
        using NpgsqlDataReader reader = getJobRequestCommand.ExecuteReader();
        while (reader.Read())
        {
            jobRequests.Add(new() { UserId = reader.GetInt16(0), Image = reader.GetString(1), Name = reader.GetString(2), Email = reader.GetString(3), MobileNumber = reader.GetString(5), Gender = EnumHelper.GetGender(reader.GetString(6)), BirthDate = reader.GetDateTime(7), Address = reader.GetString(8) });
        }
        return jobRequests;
    }

    public List<Teacher.TeacherDetails> GetTeacherList()
    {
        List<Teacher.TeacherDetails> teacherDetails = [];
        NpgsqlCommand getAdmissionRequestCommand = new("SELECT c_teacher_id, c_image, c_name, c_standard, c_qualification, c_email, c_mobile_number, c_gender, c_birth_date, c_address, c_joining_date, c_working FROM t_teachers inner join t_users ON t_teachers.c_user_id = t_users.c_user_id", connection);
        using NpgsqlDataReader reader = getAdmissionRequestCommand.ExecuteReader();
        while (reader.Read())
        {
            teacherDetails.Add(new() { TeacherId = reader.GetInt32(0), Image = reader.GetString(1), Name = reader.GetString(2), Standard = reader.IsDBNull(3) ? null : reader.GetString(3), Qualification = reader.GetString(4), Email = reader.GetString(5), MobileNumber = reader.GetString(6), Gender = EnumHelper.GetGender(reader.GetString(7)), BirthDate = reader.GetDateTime(8), Address = reader.GetString(9), JoiningDate = reader.GetDateTime(10), Working = reader.GetBoolean(11) });
        }
        return teacherDetails;
    }

    public List<Teacher.TeacherDetails> GetWorkingTeacherList()
    {
        List<Teacher.TeacherDetails> teacherDetails = [];
        NpgsqlCommand getAdmissionRequestCommand = new("SELECT c_teacher_id, c_image, c_name, c_standard, c_qualification, c_email, c_mobile_number, c_gender, c_birth_date, c_address, c_joining_date, c_working FROM t_teachers inner join t_users ON t_teachers.c_user_id = t_users.c_user_id WHERE t_teachers.c_working = true", connection);
        using NpgsqlDataReader reader = getAdmissionRequestCommand.ExecuteReader();
        while (reader.Read())
        {
            teacherDetails.Add(new() { TeacherId = reader.GetInt32(0), Image = reader.GetString(1), Name = reader.GetString(2), Standard = reader.IsDBNull(3) ? null : reader.GetString(3), Qualification = reader.GetString(4), Email = reader.GetString(5), MobileNumber = reader.GetString(6), Gender = EnumHelper.GetGender(reader.GetString(7)), BirthDate = reader.GetDateTime(8), Address = reader.GetString(9), JoiningDate = reader.GetDateTime(10), Working = reader.GetBoolean(11) });
        }
        return teacherDetails;
    }

    public void HireTeacher(Teacher.HireTeacher teacher)
    {
        NpgsqlCommand verifyStudentCommand = new("UPDATE t_users SET c_verified = true WHERE c_user_id = @userid", connection);
        verifyStudentCommand.Parameters.AddWithValue("userid", teacher.UserId);
        verifyStudentCommand.ExecuteNonQuery();

        NpgsqlCommand admitStudentCommand = new("INSERT INTO t_teachers(c_user_id, c_standard, c_qualification, c_joining_date, c_working) VALUES(@userid, @standard, @qualification, @joiningdate, @working)", connection);
        admitStudentCommand.Parameters.AddWithValue("userid", teacher.UserId);
        admitStudentCommand.Parameters.AddWithValue("standard", string.IsNullOrEmpty(teacher.Standard) ? DBNull.Value : teacher.Standard);
        admitStudentCommand.Parameters.AddWithValue("qualification", teacher.Qualification);
        admitStudentCommand.Parameters.AddWithValue("joiningdate", teacher.JoiningDate);
        admitStudentCommand.Parameters.AddWithValue("working", teacher.Working);
        admitStudentCommand.ExecuteNonQuery();
    }

    public void UpdateTeacherDetails(Teacher.AdminUpdate teacherDetails)
    {
        NpgsqlCommand updateStudentCommand = new("UPDATE t_teachers SET c_standard = @standard, c_working = @working WHERE c_teacher_id = @teacherid", connection);
        updateStudentCommand.Parameters.AddWithValue("standard", string.IsNullOrEmpty(teacherDetails.Standard) ? DBNull.Value : teacherDetails.Standard);
        updateStudentCommand.Parameters.AddWithValue("working", teacherDetails.Working);
        updateStudentCommand.Parameters.AddWithValue("teacherid", teacherDetails.TeacherId);
        updateStudentCommand.ExecuteNonQuery();
    }

    public List<TeacherwithStudent> GetTeacherswithStudents()
    {
        List<TeacherwithStudent> teacherswithStudents = [];
        NpgsqlCommand getTeacherwithStudents = new("SELECT * FROM (SELECT CAST(c_standard as int) as c_id, c_image, c_name, c_standard, c_mobile_number, c_email, c_joining_date as c_date, null as c_parent_id, c_role FROM t_users tu INNER JOIN t_teachers tt ON tu.c_user_id = tt.c_user_id WHERE c_standard != 'null' UNION SELECT tu.c_user_id, c_image, c_name, c_standard, c_mobile_number, c_email, c_admission_date as c_date, c_standard as c_parent_id, c_role FROM t_users tu INNER JOIN t_students ts ON tu.c_user_id = ts.c_user_id) tmp ORDER BY c_standard, c_role", connection);
        using NpgsqlDataReader reader = getTeacherwithStudents.ExecuteReader();
        while (reader.Read())
        {
            teacherswithStudents.Add(new() { Id = reader.GetInt16(0), Image = reader.GetString(1), Name = reader.GetString(2), Standard = reader.GetString(3), MobileNumber = reader.GetString(4), EmailAddress = reader.GetString(5), JoiningDate = reader.GetDateTime(6), ParentId = reader.IsDBNull(7) ? null : reader.GetString(7) });
        }
        return teacherswithStudents;
    }

    public void RejectCandidate(int userid)
    {
        NpgsqlCommand rejectCandidateCommand = new("DELETE FROM t_users WHERE c_user_id = @id", connection);
        rejectCandidateCommand.Parameters.AddWithValue("id", userid);
        rejectCandidateCommand.ExecuteNonQuery();
    }

    public bool UpdateTeacherProfile(User.UpdateDetails updateDetails)
    {
        try
        {

            string? imagePath = null;

            if (updateDetails.Image != null)
            {
                string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UserData");
                if (!Directory.Exists(imagesDirectory))
                {
                    Directory.CreateDirectory(imagesDirectory);
                }

                string fileName = $"{updateDetails.UserId}{Path.GetExtension(updateDetails.Image.FileName)}";
                imagePath = Path.Combine("UserData", fileName);
                string fullImagePath = Path.Combine(imagesDirectory, fileName);

                using var stream = new FileStream(fullImagePath, FileMode.Create);
                updateDetails.Image.CopyTo(stream);

            }


            var query = @"
        UPDATE t_users 
        SET 
            c_name = @Name,
            c_email = @Email,
            c_mobile_number = @MobileNumber,
            c_gender = @Gender,
            c_birth_date = @BirthDate,
            c_address = @Address,
            c_image = COALESCE(@ImagePath, c_image)
        WHERE 
            c_user_id = @UserId";

            using var command = new NpgsqlCommand(query, connection);
            command.Parameters.AddWithValue("UserId", updateDetails.UserId);
            command.Parameters.AddWithValue("Name", updateDetails.Name);
            command.Parameters.AddWithValue("Email", updateDetails.Email);
            command.Parameters.AddWithValue("MobileNumber", updateDetails.MobileNumber);
            command.Parameters.AddWithValue("Gender", updateDetails.Gender.ToString());
            command.Parameters.AddWithValue("BirthDate", updateDetails.BirthDate);
            command.Parameters.AddWithValue("Address", updateDetails.Address);
            command.Parameters.AddWithValue("ImagePath", (object?)imagePath ?? DBNull.Value);

            int rowsAffected = command.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to update student profile", ex);
        }
    }
}
