using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace MVC.Controllers;

public class AdminController : Controller
{
    string connectionString = "";
    NpgsqlConnection con;

    public AdminController(IConfiguration configuration)
    {
        connectionString = configuration.GetConnectionString("DefaultConnection")!;
        con = new NpgsqlConnection(connectionString);
    }

    public IActionResult Index() => View();
    public IActionResult ManageStandard() => View();
    public IActionResult ManageFeeStructure() => View();
    public IActionResult ManageSchoolInfo() => View();
    public IActionResult ViewFeedback() => View();
    public IActionResult ManageSubjects() => View();
    public IActionResult ManageClasswiseSubjects() => View();
    public IActionResult StudentDetails() => View();
    public IActionResult TeacherDetails() => View();
    public IActionResult HireTeachers() => View();
    public IActionResult AdmitStudents() => View();
    public IActionResult ViewSyllabus() => View();

    [HttpGet]
    [Route("Admin/GetSyllabusTimelineData/{cwsid}")]
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
    [Route("Admin/GetSubjects")]
    public JsonResult GetSubjects()
    {
        var subjects = new List<Subject>();

        using (var connection = new NpgsqlConnection(connectionString))
        {
            connection.Open();

            using (var command = new NpgsqlCommand("SELECT c_subject_name,c_subject_id FROM t_subjects ORDER BY c_subject_name;", connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    subjects.Add(new Subject
                    {
                        Id = (int)reader["c_subject_id"],
                        Name = reader["c_subject_name"].ToString(),
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
    [Route("Admin/GetSyllabusData/{cwsid}")]
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
    [Route("Admin/CreateSyllabus/{cwsid}")]
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
}