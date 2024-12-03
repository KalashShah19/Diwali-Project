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

    public List<User.UpdateDetails> GetUpdateProfileListById(int userId)
    {

        List<User.UpdateDetails> updateStudentDetails = new List<User.UpdateDetails>();


        string query = "SELECT c_user_id,c_image, c_name, c_email, c_mobile_number, c_gender, c_birth_date, c_address FROM t_users WHERE c_user_id = @userId";

        using (NpgsqlCommand GetUpdateProfileListByIdCommand = new NpgsqlCommand(query, connection))
        {

            GetUpdateProfileListByIdCommand.Parameters.AddWithValue("@userId", userId);


            using (NpgsqlDataReader reader = GetUpdateProfileListByIdCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    updateStudentDetails.Add(new User.UpdateDetails
                    {
                        UserId = reader.GetInt32(0),
                        Imagepath = reader.GetString(1),
                        Name = reader.GetString(2),
                        Email = reader.GetString(3),
                        MobileNumber = reader.GetString(4),
                        Gender = EnumHelper.GetGender(reader.GetString(5)),
                        BirthDate = reader.GetDateTime(6),
                        Address = reader.GetString(7),
                    });
                }
            }
        }
        return updateStudentDetails;
    }


    public List<Student.StudentDetails> GetStudentsListByStandard(int standard)
    {
        List<Student.StudentDetails> studentDetails = [];
        NpgsqlCommand getAdmissionRequestCommand = new("SELECT c_enrollment_number, c_image, c_name, c_standard, c_email, c_mobile_number, c_gender, c_birth_date, c_address, c_admission_date, c_studying FROM t_students inner join t_users ON t_students.c_user_id = t_users.c_user_id WHERE c_standard = @standard", connection);
        getAdmissionRequestCommand.Parameters.AddWithValue("standard", standard.ToString());
        using NpgsqlDataReader reader = getAdmissionRequestCommand.ExecuteReader();
        while (reader.Read())
        {
            studentDetails.Add(new() { EnrollmentNumber = reader.GetInt64(0), Image = reader.GetString(1), Name = reader.GetString(2), Standard = reader.GetString(3), Email = reader.GetString(4), MobileNumber = reader.GetString(5), Gender = EnumHelper.GetGender(reader.GetString(6)), BirthDate = reader.GetDateTime(7), Address = reader.GetString(8), AdmissionDate = reader.GetDateTime(9), Studying = reader.GetBoolean(10) });
        }
        return studentDetails;
    }

    public bool UpdateStudentProfile(User.UpdateDetails updateDetails)
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


    public void UpdateStudentDetails(Student.AdminUpdate studentDetails)
    {
        NpgsqlCommand updateStudentCommand = new("UPDATE t_students SET c_standard = @standard, c_studying = @studying WHERE c_enrollment_number = @enroll", connection);
        updateStudentCommand.Parameters.AddWithValue("standard", studentDetails.Standard);
        updateStudentCommand.Parameters.AddWithValue("studying", studentDetails.Studying);
        updateStudentCommand.Parameters.AddWithValue("enroll", studentDetails.EnrollmentNumber);
        updateStudentCommand.ExecuteNonQuery();
    }
}