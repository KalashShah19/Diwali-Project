using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace MVC.Controllers
{
    public class StudentController : Controller
    {

        string connectionString = "";
        NpgsqlConnection con;

        public StudentController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            con = new NpgsqlConnection(connectionString);
        }

        public IActionResult Index() => View("UpdateProfile");
        public IActionResult SchoolInfo() => View();
        public IActionResult UpdateProfile() => View();
        public IActionResult StudentTimeTable() => View();
        public IActionResult StudentFeesDetails() => View();
        public IActionResult ShowStudentFeedbacks() => View();
        public IActionResult ShowSyllabus() => View();

        [HttpGet]
        public IActionResult FeedbackByStudent()
        {
            ViewBag.UserID = 101;
            return View();
        }

        [HttpGet]
        [Route("GetStudentClass/{userID}")]

        public JsonResult GetStudentClass(int userID)
        {
            int studentClass = 0;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT Class FROM Student WHERE userID = @userID AND studying = true;", connection))
                {
                    command.Parameters.AddWithValue("@userID", userID);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            studentClass = (int)reader["Class"]!;
                        }
                    }
                }
            }

            if (studentClass == 0)
            {
                return Json(new { success = false, message = "Student not found or not currently studying." });
            }

            return Json(new { success = true, studentClass });
        }

        [HttpGet]
        public JsonResult GetSubjects()
        {
            int id = 142;
            long teacherId;
            var subjects = new List<Subject>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("select c_teacher_id from t_teachers where c_user_id = @id;", connection);
                command.Parameters.AddWithValue("@id", id!);
                teacherId = (long)command.ExecuteScalar()!;
                // Console.WriteLine("teacher id = "+ teacherId);

                using (command = new NpgsqlCommand("SELECT DISTINCT c_subject_id, c_subject_name FROM t_classwise_subjects NATURAL JOIN t_subjects WHERE c_teacher_id = @teacherid ORDER BY c_subject_name;", connection))
                {
                    command.Parameters.AddWithValue("@teacherid", teacherId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subjects.Add(new Subject
                            {
                                Id = (int)reader["c_subject_id"],
                                Name = (string)reader["c_subject_name"]
                            });
                        }
                    }
                }
            }
            return Json(subjects);
        }

        [HttpGet]
        public JsonResult GetCWSID(int subjectId, string standard)
        {
            int cwsid = 0;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT c_id FROM t_ClassWise_Subjects WHERE c_Subject_ID = @SubjectID AND c_Standard = @Standard;", connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.Parameters.AddWithValue("@Standard", standard);
                    cwsid = (int)command.ExecuteScalar()!;
                }
            }
            return Json(new { cwsid });
        }

        [HttpGet]
        [Route("Student/GetSyllabusTimelineData/{cwsid}")]
        public JsonResult GetSyllabusTimelineData(int cwsid)
        {
            var syllabusData = new List<Syllabus>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM t_Syllabus WHERE c_CWSID = @cwsid ORDER BY c_start ASC;", connection))
                {
                    command.Parameters.AddWithValue("@cwsid", cwsid);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            syllabusData.Add(new Syllabus
                            {
                                SyllabusID = (int)reader["c_SyllabusID"],
                                CWSID = (int)reader["c_CWSID"],
                                title = (string)reader["c_chapterName"],
                                Start = (DateTime)reader["c_start"],
                                End = (DateTime)reader["c_end"],
                                percentComplete = (double)reader["c_completed"]
                            });
                        }
                    }
                }
            }

            var Data = syllabusData.Select(s => new
            {
                title = s.title,
                subtitle = s.Start.ToString("MMMM dd, yyyy"),
                date = s.Start,
                description = s.percentComplete * 100 + "% Completed",
            }).ToList();

            return Json(Data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}