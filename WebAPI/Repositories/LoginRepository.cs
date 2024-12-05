using System.Security.Cryptography;
using System.Text;
using Npgsql;
using WebAPI.Libraries;
// using WebAPI.Libraries;

using WebAPI.Models;

// using Microsoft.AspNetCore.Identity;


namespace WebAPI.Repositories;

public class LoginRepository : ILoginRepository
{
    private readonly NpgsqlConnection connection;
    private readonly Mailer mailer;

    public LoginRepository(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString(
            "POSTGRESQL_CONNECTION_STRING"
        );
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("PostgreSQL connection string is not defined.");
        NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
        this.connection = dataSource.OpenConnection();

        mailer = new();
    }

    public (int?,string?) ValidateUser(Login Login)
    {
        try
        {
            if (connection.State == System.Data.ConnectionState.Closed)
            {
                connection.Open();
            }

            Login.Password = Convert.ToBase64String(
                SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(Login.Password))
            );

            Console.WriteLine(Login.Email);
            Console.WriteLine(Login.Password);

            NpgsqlCommand cmd =
                new(
                    "SELECT c_user_id, c_role  FROM t_users WHERE c_email = @c_email AND c_password = @password AND c_verified = true",
                    connection
                );

            // Console.WriteLine("dhruvvvvvv:" + Login.c_verified);
            cmd.Parameters.AddWithValue("@c_email", Login.Email);
            cmd.Parameters.AddWithValue("@password", Login.Password);
            NpgsqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                Console.WriteLine("verified");
                int c_user_id = dr.GetInt32(0);
                string c_role = dr.GetString(1);
                connection.Close();
                return (c_user_id, c_role);
                // int count = Convert.ToInt32(dr[0]);

                // return count > 0;
            }
            connection.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding user: {ex.Message}");
        }
        return (null,null);
    }

    public (string, string) RequestPasswordChange(string email)
    {
        try
        {
            NpgsqlCommand cmd =
                new("SELECT c_email FROM t_users WHERE c_email = @c_email", connection);
            cmd.Parameters.AddWithValue("c_email", email);
            NpgsqlDataReader dr = cmd.ExecuteReader();

            string otp = new Random().Next(100000, 999999).ToString();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    string? status = mailer.SendMail(email, Mailer.PasswordChangeMail(email, otp));
                    Console.WriteLine("inside status" + status);
                    return (status, otp);

                    // if (!string.IsNullOrEmpty(status)){
                    //     Console.WriteLine("inside");
                    //     return (status, otp);
                    // }
                }
            }
            return (null, null);
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return (e.Message, "");
        }
    }

    public string? Newpassword(Forgotpassword ForgotPassword)
    {
        try
        {
            Console.WriteLine(ForgotPassword.ConfirmPassword);
            Console.WriteLine(ForgotPassword.Email);
            Console.WriteLine(ForgotPassword.Password);
            Console.WriteLine(ForgotPassword.OTP);

            // string updateMessage = "Password updated successfully.";

          
            using (
                NpgsqlCommand cmd =
                    new("SELECT c_email FROM t_users WHERE c_email = @c_email", connection)
            )
            {
                cmd.Parameters.AddWithValue("c_email", ForgotPassword.Email);
                using (NpgsqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        return "Email not found.";
                    }
                }
            }
           
            using (
                NpgsqlCommand updateCmd =
                    new(
                        "UPDATE t_users SET c_password = @c_password WHERE c_email = @c_email",connection)
            )
            {
                ForgotPassword.Password = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(ForgotPassword.Password))
                );

                updateCmd.Parameters.AddWithValue("c_password", ForgotPassword.Password);
                updateCmd.Parameters.AddWithValue("c_email", ForgotPassword.Email);

                int rowsAffected = updateCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    return "Password updated successfully.";
                }
                else
                {
                    return "Password update failed.";
                }
            }
        }
        catch (Exception e)
        {
            Console.Write(e.StackTrace);
            return e.Message;
        }
    }
}
