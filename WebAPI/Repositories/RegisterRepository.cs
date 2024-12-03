using Npgsql;
using WebAPI.Models;
using WebAPI.Libraries;
using System.Text;
using System.Security.Cryptography;


namespace WebAPI.Repositories
{
    public class RegisterRepository : IRegisterRepository
    {
        private readonly NpgsqlConnection connection;

        public RegisterRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
            NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
            this.connection = dataSource.OpenConnection();
        }

        public async Task<bool> RegisterUser(User.Register registerModel)
        {
            try
            {

                using (var checkCmd = new NpgsqlCommand("SELECT COUNT(c_email) FROM t_users WHERE c_email = @c_email", connection))
                {
                    checkCmd.Parameters.AddWithValue("@c_email", registerModel.Email);
                    var count = await checkCmd.ExecuteScalarAsync();

                    if (Convert.ToInt32(count) > 0)
                    {
                        return false;
                    }
                }

                DateTime validBirthDate = registerModel.BirthDate > DateTime.MinValue ? registerModel.BirthDate : DateTime.Today;

                registerModel.Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(registerModel.Password)));
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO t_users (c_name, c_email, c_password, c_mobile_number, c_gender, c_birth_date, c_address, c_role, c_verified) " +
                    "VALUES (@c_name, @c_email, @c_password, @c_mobile_number, @c_gender, @c_birth_date, @c_address, @c_role, @c_verified) RETURNING c_user_id", connection))
                {
                    cmd.Parameters.AddWithValue("@c_name", registerModel.Name);
                    cmd.Parameters.AddWithValue("@c_email", registerModel.Email);
                    cmd.Parameters.AddWithValue("@c_password", registerModel.Password);
                    cmd.Parameters.AddWithValue("@c_mobile_number", registerModel.MobileNumber);
                    cmd.Parameters.AddWithValue("@c_gender", registerModel.Gender.ToString());
                    cmd.Parameters.AddWithValue("@c_birth_date", validBirthDate);
                    cmd.Parameters.AddWithValue("@c_address", registerModel.Address);
                    cmd.Parameters.AddWithValue("@c_role", registerModel.Role.ToString());
                    cmd.Parameters.AddWithValue("@c_verified", registerModel.Verified);

                    var userId = await cmd.ExecuteScalarAsync();
                    if (userId != null)
                    {

                        string imagePath = FileHandler.UserProfilePicture(registerModel.ProfileImage, userId.ToString());


                        using (var updateCmd = new NpgsqlCommand("UPDATE t_users SET c_image = @c_image WHERE c_user_id = @c_user_id", connection))
                        {
                            updateCmd.Parameters.AddWithValue("@c_image", imagePath);
                            updateCmd.Parameters.AddWithValue("@c_user_id", userId);
                            await updateCmd.ExecuteNonQueryAsync();
                        }

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
            }

            return false;
        }

        public async Task<bool> RoleStudent(string email, string standard) // Updated to match interface
        {
            try
            {
                int? userId = null;
                using (var checkCmd = new NpgsqlCommand("SELECT c_user_id FROM t_users WHERE c_email = @c_email", connection))
                {
                    checkCmd.Parameters.AddWithValue("@c_email", email);
                    var result = await checkCmd.ExecuteScalarAsync();
                    Console.WriteLine($"Email Check Result: {result}");

                    if (result != null && result != DBNull.Value)
                    {
                        userId = Convert.ToInt32(result);
                        Console.WriteLine($"User ID found: {userId}");
                    }
                    else
                    {
                        Console.WriteLine("Email not found in t_users.");
                        return false;
                    }
                }


                using (var insertCmd = new NpgsqlCommand(
                    "INSERT INTO t_students (c_user_id, c_standard, c_admission_date,c_studying) VALUES (@c_user_id, @c_standard, @c_admission_date, @c_studying)", connection))
                {
                    insertCmd.Parameters.AddWithValue("@c_user_id", userId.Value);
                    insertCmd.Parameters.AddWithValue("@c_standard", standard);
                    insertCmd.Parameters.AddWithValue("@c_admission_date", DateTime.Today);
                    insertCmd.Parameters.AddWithValue("@c_studying", false);

                    int result = await insertCmd.ExecuteNonQueryAsync();
                    return result > 0;  // If rows are affected, insertion was successful
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RoleStudent: {ex.Message}");
            }

            return false;
        }
        public async Task<bool> RoleTeacher(string email, string standard, string qualification)
        {
            try
            {

                int? userId = null;
                using (var checkCmd = new NpgsqlCommand("SELECT c_user_id FROM t_users WHERE c_email = @c_email", connection))
                {
                    checkCmd.Parameters.AddWithValue("@c_email", email);
                    var result = await checkCmd.ExecuteScalarAsync();

                    if (result != null)
                    {
                        userId = Convert.ToInt32(result);
                    }
                    else
                    {

                        return false;
                    }
                }


                using (var insertCmd = new NpgsqlCommand(
                    "INSERT INTO t_teachers (c_user_id, c_standard, c_qualification, c_working, c_joining_date) VALUES (@c_user_id, @c_standard, @c_qualification, @c_working, @c_joining_date)", connection))
                {
                    insertCmd.Parameters.AddWithValue("@c_user_id", userId.Value);
                    insertCmd.Parameters.AddWithValue("@c_standard", standard);
                    insertCmd.Parameters.AddWithValue("@c_qualification", qualification);
                    insertCmd.Parameters.AddWithValue("@c_working", false);
                    insertCmd.Parameters.AddWithValue("@c_joining_date", DateTime.Today);

                    int result = await insertCmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RoleTeacher: {ex.Message}");
            }

            return false;
        }

    }
}
