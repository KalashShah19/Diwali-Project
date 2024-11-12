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

        [HttpGet]
        public JsonResult GetSyllabusData()
        {
            var syllabusData = new List<Syllabus>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("SELECT * FROM t_Syllabus", connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        syllabusData.Add(new Syllabus
                        {
                            SyllabusID = (int)reader["c_SyllabusID"],
                            CWSID = (int)reader["c_CWSID"],
                            ChapterName = (string)reader["c_chapterName"],
                            Start = (DateTime)reader["c_start"],
                            End = (DateTime)reader["c_end"],
                            Completed = (string)reader["c_completed"]
                        });
                    }
                }
            }

            var formattedData = syllabusData.Select(s => new
            {
                s.SyllabusID,
                s.CWSID,
                s.ChapterName,
                Start = s.Start.ToString("yyyy/MM/dd H:mm"),
                End = s.End.ToString("yyyy/MM/dd H:mm"),
                s.Completed,
            });

            return Json(formattedData);
        }

        [HttpPost]
        public JsonResult CreateSyllabus(Syllabus syllabus)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO t_Syllabus (c_CWSID, c_chapterName, c_start, c_end, c_completed) VALUES (@CWSID, @ChapterName, @Start, @End, @Completed) RETURNING c_SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@CWSID", 0);
                    command.Parameters.AddWithValue("@ChapterName", "");
                    command.Parameters.AddWithValue("@Start", DateTime.Now);
                    command.Parameters.AddWithValue("@End", DateTime.Now);
                    command.Parameters.AddWithValue("@Completed", "0");
                    syllabus.SyllabusID = (int)command.ExecuteScalar()!;
                }
            }
            return Json(syllabus);
        }

        [HttpPost]
        public JsonResult UpdateSyllabus(Syllabus syllabus)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("UPDATE t_Syllabus SET c_CWSID = @CWSID, c_chapterName = @ChapterName, c_startDate = @Start, c_end = @EndDate, c_completed = @Completed WHERE c_SyllabusID = @SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@CWSID", syllabus.CWSID);
                    command.Parameters.AddWithValue("@ChapterName", syllabus.ChapterName!);
                    command.Parameters.AddWithValue("@Start", syllabus.Start!);
                    command.Parameters.AddWithValue("@End", syllabus.End!);
                    command.Parameters.AddWithValue("@Completed", syllabus.Completed!);
                    command.Parameters.AddWithValue("@SyllabusID", syllabus.SyllabusID);
                    command.ExecuteNonQuery();
                }
            }
            return Json(syllabus);
        }

        [HttpPost]
        public JsonResult DeleteSyllabus(int SyllabusID)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("DELETE FROM t_Syllabus WHERE c_SyllabusID = @SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@SyllabusID", SyllabusID);
                    command.ExecuteNonQuery();
                }
            }
            return Json(SyllabusID);
        }
    }
}