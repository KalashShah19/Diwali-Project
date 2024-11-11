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

        public JsonResult GetSyllabusData()
        {
            var syllabusData = new List<Syllabus>();

            con.Open();
            using (var command = new NpgsqlCommand("SELECT * FROM t_syllabus", con))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    syllabusData.Add(new Syllabus
                    {
                        SyllabusID = (int)reader["c_SyllabusID"],
                        CWSID = (int)reader["c_CWSID"],
                        ChapterName = (string)reader["c_ChapterName"],
                        StartDate = (DateTime)reader["c_StartDate"],
                        EndDate = (DateTime)reader["c_EndDate"],
                        Status = (string)reader["c_Status"]
                    });
                }
            }
            return Json(syllabusData);
        }

        [HttpPost]
        public JsonResult CreateSyllabus(Syllabus syllabus)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("INSERT INTO t_syllabus (c_CWSID, c_ChapterName, c_StartDate, c_EndDate, c_Status) VALUES (@CWSID, @ChapterName, @StartDate, @EndDate, @Status) RETURNING SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@CWSID", syllabus.CWSID);
                    command.Parameters.AddWithValue("@ChapterName", syllabus.ChapterName!);
                    command.Parameters.AddWithValue("@StartDate", syllabus.StartDate);
                    command.Parameters.AddWithValue("@EndDate", syllabus.EndDate);
                    command.Parameters.AddWithValue("@Status", syllabus.Status!);
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
                using (var command = new NpgsqlCommand("UPDATE t_syllabus SET c_CWSID = @CWSID, c_ChapterName = @ChapterName, c_StartDate = @StartDate, c_EndDate = @EndDate, c_Status = @Status WHERE c_SyllabusID = @SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@CWSID", syllabus.CWSID);
                    command.Parameters.AddWithValue("@ChapterName", syllabus.ChapterName!);
                    command.Parameters.AddWithValue("@StartDate", syllabus.StartDate);
                    command.Parameters.AddWithValue("@EndDate", syllabus.EndDate);
                    command.Parameters.AddWithValue("@Status", syllabus.Status!);
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
                using (var command = new NpgsqlCommand("DELETE FROM t_syllabus WHERE c_SyllabusID = @SyllabusID", connection))
                {
                    command.Parameters.AddWithValue("@SyllabusID", SyllabusID);
                    command.ExecuteNonQuery();
                }
            }
            return Json(SyllabusID);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}