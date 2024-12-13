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
    //[Route("[controller]")]
    public class TeacherController : Controller
    {
        string connectionString = "";
        NpgsqlConnection con;

        public TeacherController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            con = new NpgsqlConnection(connectionString);
        }

        public IActionResult Index(int c_user_id)
        {
            HttpContext.Session.SetString("c_user_id", c_user_id.ToString());
            return View("Timetable");
        }

        public IActionResult Timetable1()
        {
            return View("Timetable");
        }

        public IActionResult TeacherTimetable()
        {
            return View("TeacherTimetable");
        }

        public IActionResult TeacherDashboard()
        {

            return View("TeacherDashboard");
        }
        public IActionResult Feedbacks() => View();
        public IActionResult Material() => View();
        public IActionResult UpdateProfile() => View();

        public IActionResult ManageSyllabus() => View("Gantt");

        [HttpGet("GetStudentClass/{userID}")]
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
        [Route("Teacher/GetSyllabusTimelineData/{cwsid}")]
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

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
        public JsonResult GetStandards()
        {
            var standards = new List<Standard>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT c_standard FROM t_standards ORDER BY c_standard;", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        standards.Add(new Standard
                        {
                            standard = reader["c_standard"].ToString(),
                        });
                    }
                }
            }

            return Json(standards);
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
        [Route("Teacher/GetSyllabusData/{cwsid}")]
        public JsonResult GetSyllabusData(int cwsid)
        {
            Console.WriteLine("inside ");
            int id = 142;
            // int? id = HttpContext.Session.GetInt32("user_id");
            long teacherId = 0;
            var syllabusData = new List<Syllabus>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("select c_teacher_id from t_teachers where c_user_id = @id;", connection);
                command.Parameters.AddWithValue("@id", id!);
                teacherId = (long)command.ExecuteScalar()!;

                using (command = new NpgsqlCommand("SELECT * FROM t_Syllabus WHERE c_CWSID = @cwsid ORDER BY c_start ASC;", connection))
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

            var data = syllabusData.Select(s => new
            {
                s.SyllabusID,
                s.CWSID,
                s.title,
                Start = s.Start.ToString("yyyy/MM/dd H:mm"),
                End = s.End.ToString("yyyy/MM/dd H:mm"),
                s.percentComplete,
                done = s.percentComplete * 100
            });

            return Json(data);
        }

        [HttpGet]
        [Route("Teacher/CreateSyllabus/{cwsid}")]
        public IActionResult CreateSyllabus(int cwsid)
        {

            Console.WriteLine("creating");
            Console.WriteLine("cwsid = " + cwsid);
            // int? id = HttpContext.Session.GetInt32("user_id");
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO t_Syllabus (c_CWSID, c_chapterName, c_start, c_end, c_completed) VALUES (@CWSID, @ChapterName, @Start, @End, @Completed) RETURNING c_SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@CWSID", cwsid);
                    command.Parameters.AddWithValue("@ChapterName", "");
                    command.Parameters.AddWithValue("@Start", DateTime.Now);
                    command.Parameters.AddWithValue("@End", DateTime.Now);
                    command.Parameters.AddWithValue("@Completed", 0);
                    int SyllabusID = (int)command.ExecuteScalar()!;
                    if (SyllabusID == 1)
                    {
                        return Ok("Syllabus Created");
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult UpdateSyllabus(Syllabus syllabus)
        {
            syllabus.Start = syllabus.Start == DateTime.MinValue ? DateTime.Now : syllabus.Start;
            syllabus.End = syllabus.End == DateTime.MinValue ? DateTime.Now.AddDays(7) : syllabus.End;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE t_Syllabus SET c_chapterName = @ChapterName, c_start = @Start, c_end = @End, c_completed = @Completed WHERE c_SyllabusID = @SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@ChapterName", syllabus.title!);
                    command.Parameters.AddWithValue("@Start", syllabus.Start!);
                    command.Parameters.AddWithValue("@End", syllabus.End!);
                    command.Parameters.AddWithValue("@Completed", syllabus.percentComplete!);
                    command.Parameters.AddWithValue("@SyllabusID", syllabus.SyllabusID);
                    command.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteSyllabus(int SyllabusID)
        {
            Console.WriteLine("id = " + SyllabusID);
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM t_Syllabus WHERE c_SyllabusID = @SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@SyllabusID", SyllabusID);
                    command.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

    }
}