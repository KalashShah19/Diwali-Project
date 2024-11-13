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
    public class MVCSyllabusController : Controller
    {
        string connectionString = "";
        NpgsqlConnection con;

        public MVCSyllabusController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
            con = new NpgsqlConnection(connectionString);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View("Test");
        }

        public IActionResult Syllabus()
        {
            return View("TimeLine");
        }

        [HttpGet]
        [Route("MVCSyllabus/GetSyllabusTimelineData/{cwsid}")]
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

            var data = syllabusData.Select(s => new
            {
                s.SyllabusID,
                s.title,
                Start = s.Start.ToString("yyyy/MM/dd H:mm"),
                End = s.End.ToString("yyyy/MM/dd H:mm"),
                s.percentComplete
            });

            return Json(data);
        }

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [HttpGet]
        public JsonResult GetCWSID(int subjectId, string standard)
        {
            int? cwsid = null;

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new NpgsqlCommand("SELECT c_id FROM t_ClassWise_Subjects WHERE c_Subject_ID = @SubjectID AND c_Standard = @Standard;", connection))
                {
                    command.Parameters.AddWithValue("@SubjectID", subjectId);
                    command.Parameters.AddWithValue("@Standard", standard);

                    cwsid = (int?)command.ExecuteScalar();
                }
            }
            return Json(new { cwsid });
        }

        [HttpGet]
        [Route("MVCSyllabus/GetSyllabusData/{cwsid}")]
        public JsonResult GetSyllabusData(int cwsid)
        {
            int id = 0;
            // int? id = HttpContext.Session.GetInt32("user_id");
            Console.WriteLine("cwsid = " + cwsid);
            var syllabusData = new List<Syllabus>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("select c_teacher_id from t_teachers where c_user_id = @id;", connection);
                command.Parameters.AddWithValue("@id", id!);
                int teacherId = (int)command.ExecuteScalar()!;

                // using (var command = new NpgsqlCommand("SELECT * FROM t_Syllabus WHERE c_CWSID = @cwsid and c_teacher_id = @teacherid ORDER BY c_start ASC;", connection))
                using (command = new NpgsqlCommand("SELECT * FROM t_Syllabus WHERE c_CWSID = @cwsid ORDER BY c_start ASC;", connection))
                {
                    command.Parameters.AddWithValue("@cwsid", cwsid);
                    // command.Parameters.AddWithValue("@teacherid", teacherid);

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
        public JsonResult GetSubjects()
        {
            // List to hold subject data
            var subjects = new List<Subject>();

            // Connect to the database and fetch subjects (assuming you have a "t_subjects" table)
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();

                // Query to get subjects (you can adjust the query as per your actual table structure)
                using (var command = new NpgsqlCommand("SELECT c_Subject_ID, c_Subject_Name FROM t_subjects ORDER BY c_Subject_Name;", connection))
                using (var reader = command.ExecuteReader())
                {
                    // Read data and populate the subject list
                    while (reader.Read())
                    {
                        subjects.Add(new Subject
                        {
                            Id = (int)reader["c_Subject_ID"],
                            Name = (string)reader["c_Subject_Name"]
                        });
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

        [HttpPost]
        public IActionResult CreateSyllabus(Syllabus syllabus)
        {
            int id = 0;
            // int? id = HttpContext.Session.GetInt32("user_id");
            int cwsid = 0;
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var command = new NpgsqlCommand("select c_teacher_id from t_teachers where c_user_id = @id;", connection);
                command.Parameters.AddWithValue("@id", id!);
                int teacherId = (int)command.ExecuteScalar()!;

                using (command = new NpgsqlCommand("INSERT INTO t_Syllabus (c_CWSID, c_chapterName, c_start, c_end, c_completed) VALUES (@CWSID, @ChapterName, @Start, @End, @Completed) RETURNING c_SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@CWSID", cwsid);
                    command.Parameters.AddWithValue("@ChapterName", "");
                    command.Parameters.AddWithValue("@Start", DateTime.Now);
                    command.Parameters.AddWithValue("@End", DateTime.Now);
                    command.Parameters.AddWithValue("@Completed", 0);
                    syllabus.SyllabusID = (int)command.ExecuteScalar()!;
                }
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateSyllabus(Syllabus syllabus)
        {
            syllabus.Start = syllabus.Start == DateTime.MinValue ? DateTime.Now : syllabus.Start;
            syllabus.End = syllabus.End == DateTime.MinValue ? DateTime.Now.AddDays(7) : syllabus.End;

            Console.WriteLine("in update");
            Console.WriteLine(syllabus.SyllabusID);
            Console.WriteLine(syllabus.Start);
            Console.WriteLine(syllabus.End);
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
    }
}