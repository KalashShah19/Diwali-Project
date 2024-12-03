using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;
using WebAPI.Libraries;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories
{
    public class ClassWiseStudentRepository : IClassWiseStudentRepository
    {
        private readonly NpgsqlConnection connection;

        public ClassWiseStudentRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
            NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
            this.connection = dataSource.OpenConnection();
        }

        public int DeleteStudentDetailsWithFees(int id)
        {
            using (NpgsqlCommand deleteStudentDetailCommand = new("update t_students set c_studying=false where c_user_id=@id ", connection))
            {
                deleteStudentDetailCommand.Parameters.AddWithValue("@id", id);
                return deleteStudentDetailCommand.ExecuteNonQuery();
            }

        }

        public int updateStudentDetailsWithFees(int id, string standers, bool studying)
        {
            using (NpgsqlCommand updateStudentDetailCommand = new("update t_students set c_studying=@studying,c_standard=@standard where c_user_id=@id ", connection))
            {
                updateStudentDetailCommand.Parameters.AddWithValue("@id", id);
                updateStudentDetailCommand.Parameters.AddWithValue("@studying", studying);
                updateStudentDetailCommand.Parameters.AddWithValue("@standard", standers);
                return updateStudentDetailCommand.ExecuteNonQuery();
            }
        }

        public List<Student.StudentDetailsWithFees> StudentDetailsWithFees(int id)
        {
            int standard = 0;
            List<Student.StudentDetailsWithFees> studentDetailsWithFees = new List<Student.StudentDetailsWithFees>();

            using (NpgsqlCommand getStandersCommand = new("SELECT c_standard from t_teachers where c_user_id=@id", connection))
            {
                getStandersCommand.Parameters.AddWithValue("@id", id);
                using (NpgsqlDataReader reader = getStandersCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        standard = Convert.ToInt32(reader.GetString(0));
                    }
                    Console.WriteLine("standers = " + standard);
                }

            }

            if (standard != 0)
            {

                using (NpgsqlCommand getStudentDetailsWithFeesCommand = new("SELECT u.c_user_id,u.c_image,u.c_name,u.c_email,u.c_mobile_number,u.c_gender,u.c_birth_date,u.c_address,s.c_enrollment_number,s.c_admission_date,s.c_standard, p.c_status FROM t_students AS s JOIN t_users AS u ON u.c_user_id = s.c_user_id FULL JOIN t_payments AS p ON s.c_user_id = p.c_user_id AND p.c_currentstandard = @standard WHERE s.c_standard = @standard AND s.c_studying = true AND u.c_verified = true;", connection))
                {

                    getStudentDetailsWithFeesCommand.Parameters.AddWithValue("@standard", standard.ToString());
                    using (NpgsqlDataReader reader1 = getStudentDetailsWithFeesCommand.ExecuteReader())
                    {

                        while (reader1.Read())
                        {
                            var students = new Student.StudentDetailsWithFees
                            {
                                user_id = reader1.GetInt32(0),
                                Image = reader1.GetString(1),
                                Name = reader1.GetString(2),
                                Email = reader1.GetString(3),
                                MobileNumber = reader1.GetString(4),
                                Gender = EnumHelper.GetGender(reader1.GetString(5)),
                                BirthDate = reader1.GetDateTime(6),
                                Address = reader1.GetString(7),
                                EnrollmentNumber = reader1.GetInt32(8),
                                AdmissionDate = reader1.GetDateTime(9),
                                Standard = reader1.GetString(10),
                                // Studying = reader1.GetBoolean(11),
                                FeeStatus = reader1.IsDBNull(11) ? "No Payment Info" : reader1.GetString(11)

                            };
                            studentDetailsWithFees.Add(students);
                        }
                    }
                }



            }
            return studentDetailsWithFees;

        }
    }
}