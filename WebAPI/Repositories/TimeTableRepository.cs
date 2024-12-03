using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo_MVC.Models;
using Npgsql;
using WebAPI.Models;
using WebAPI.Repositories.Interfaces;

namespace WebAPI.Repositories
{
    public class TimeTableRepository : ITimeTableRepository
    {
        private readonly NpgsqlConnection connection;

        public TimeTableRepository(IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("POSTGRESQL_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString)) throw new Exception("PostgreSQL connection string is not defined.");
            NpgsqlDataSource dataSource = NpgsqlDataSource.Create(connectionString);
            this.connection = dataSource.OpenConnection();
        }

        public List<Teacherinfo> GetTeacherinfos()
        {
            List<Teacherinfo> teacherinfos = new List<Teacherinfo>();
            NpgsqlCommand getTeacherinfosCommand = new("SELECT c_user_id,c_name,c_image from t_users where c_role='Teacher'", connection);
            using NpgsqlDataReader reader = getTeacherinfosCommand.ExecuteReader();
            while (reader.Read())
            {
                var teacherinfo = new Teacherinfo { userid = reader.GetInt32(0), teachername = reader.GetString(1), img = reader.GetString(2) };
                teacherinfos.Add(teacherinfo);
            }
            return teacherinfos;
        }

        public (List<string>, List<Timetable>) GetTimeTable()
        {
            List<string> standers = [];
            List<Timetable> timetables = new List<Timetable>();
            string query = "select * from t_standards";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        standers.Add(reader.GetString(0));
                    }
                }
            }
            string query1 = "select c.c_standard,s.c_subject_name,u.c_name,u.c_user_id from t_classwise_subjects as c join t_subjects as s on c.c_subject_id=s.c_subject_id  join t_teachers as t on t.c_teacher_id=c.c_teacher_id join t_users as u on u.c_user_id=t.c_user_id  order by c.c_standard ";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query1, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timetable = new Timetable
                        {
                            c_standard = Convert.ToInt32(reader.GetString(0)),
                            c_subject_name = reader.GetString(1),
                            c_name = reader.GetString(2),
                            c_user_id=reader.GetInt32(3)
                            
                        };
                        timetables.Add(timetable);
                    }
                }
            }
            return (standers, timetables);
        }

        public List<string> GetStandards(int id)
        {
            List<string> standards = [];
            NpgsqlCommand getStandardsCommand = new("select c.c_standard from t_classwise_subjects as c join t_teachers as t on t.c_teacher_id=c.c_teacher_id join t_users as u on u.c_user_id=t.c_user_id  where t.c_user_id=@id ORDER BY CAST(c.c_standard as int)", connection);
            getStandardsCommand.Parameters.AddWithValue("@id", id);
            using NpgsqlDataReader reader = getStandardsCommand.ExecuteReader();
            while (reader.Read())
            {
                standards.Add(reader.GetString(0));
            }
            return standards;
        }

        public (List<string>, List<Timetable>) GetTeacherTimetable()
        {
            List<string> standers = [];
            List<Timetable> timetables = new List<Timetable>();
            string query = "select * from t_standards";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        standers.Add(reader.GetString(0));
                    }
                }
            }
            string query1 = "select c.c_standard,s.c_subject_name,u.c_name,u.c_user_id from t_classwise_subjects as c join t_subjects as s on c.c_subject_id=s.c_subject_id  join t_teachers as t  on t.c_teacher_id=c.c_teacher_id join t_users as u on u.c_user_id=t.c_user_id order by c.c_standard";
            using (NpgsqlCommand cmd = new NpgsqlCommand(query1, connection))
            {
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var timetable = new Timetable
                        {
                            c_standard = Convert.ToInt32(reader.GetString(0)),
                            c_subject_name = reader.GetString(1),
                            c_name = reader.GetString(2),
                            c_user_id = reader.GetInt32(3)
                        };
                        timetables.Add(timetable);
                        Console.WriteLine(reader.GetInt32(3)
);
                    }

                }
            }
            return (standers, timetables);
        }
    }
}