using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kendo_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeacherController : ControllerBase
    {
        private readonly TimeTableRepository timeTable;
        private readonly ClassWiseStudentRepository classWiseStudent;
        private readonly StandardRepository standard;
        private readonly TeacherRepository teacherRepository;
        private readonly MaterialRepository _materialService;
        private readonly FeedbackRepository _feedbackRepository;
        public TeacherController(IConfiguration configuration)
        {
            timeTable = new(configuration);
            classWiseStudent = new(configuration);
            standard = new(configuration);
            teacherRepository = new(configuration);
            _materialService = new(configuration);
            _feedbackRepository = new(configuration);
        }

        [HttpGet]
        [Route("gettecherinfo")]
        public IActionResult gettecherinfo()
        {
            Console.WriteLine("hello");
            List<Teacherinfo> teacherinfos = timeTable.GetTeacherinfos();
            return Ok(teacherinfos);
        }


        [HttpGet]
        [Route("getclassWiseStudent")]
        public IActionResult getclassWiseStudent(int id)
        {
            List<Student.StudentDetailsWithFees> studentDetailsWithFees = classWiseStudent.StudentDetailsWithFees(id);
            return Ok(studentDetailsWithFees);
        }

        [HttpDelete]
        [Route("deleteclassWiseStudent")]
        public IActionResult deleteclassWiseStudent(int id)
        {
            Console.WriteLine("delete id = " + id);
            int row = classWiseStudent.DeleteStudentDetailsWithFees(id);
            if (row > 0)
            {
                return Ok(new { Message = "Student deleted successfully" });
            }
            else
            {
                return Ok(new { Message = "Student not deleted" });
            }

        }

        [HttpPatch]
        [Route("updateclassWiseStudent")]
        public IActionResult updateclassWiseStudent([FromBody] StudentUpdateForTeacher model)
        {

            int row = classWiseStudent.updateStudentDetailsWithFees(model.id, model.standers, model.studying);
            if (row > 0)
            {
                return Ok(new { Message = "Student Update successfully" });
            }
            else
            {
                return Ok(new { Message = "Student not update" });
            }
        }

        [HttpGet]
        [Route("timetable")]
        public IActionResult timetable()
        {
            Console.WriteLine("thime table");
            (List<string> standers, List<Timetable> timetables) = timeTable.GetTimeTable();
            return Ok(new { standers, timetables });
        }

        [HttpGet]
        [Route("timetableForTeacher")]
        public IActionResult timetableForTeacher()
        {
            Console.WriteLine("thime table");
            (List<string> standers, List<Timetable> timetables) = timeTable.GetTeacherTimetable();
            return Ok(new { standers, timetables });
        }

        [HttpGet]
        [Route("Getstandards")]
        public IActionResult Getstandards()
        {
            List<string> standards = standard.GetStandards();
            return Ok(standards);
        }

        [HttpGet]
        [Route("GetstandardsForTeacher")]
        public IActionResult GetstandardsForTeacher(int id)
        {
            List<string> standards = timeTable.GetStandards(id);
            return Ok(standards);
        }



        [HttpPut("UpdateTeacherProfile/{userId}")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public IActionResult UpdateTeacherProfile(int userId, [FromForm] User.UpdateDetails profile)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid student profile data.");

            profile.UserId = userId;

            try
            {
                var isUpdated = teacherRepository.UpdateTeacherProfile(profile);
                if (isUpdated)
                    return Ok("Student profile updated successfully.");
                else
                    return NotFound("Student not found.");
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }


        //     >>>>>>>       CHANGES BY PRITAM    <<<<<<<<<
        [HttpGet("GetFeedBacks")]
        public IActionResult GetFeedBacks(int id)
        {
            var list = _feedbackRepository.GetFeedbacks(id);
            if (list != null)
            {
                return Ok(list);
            }
            return BadRequest("Cannot get feedbacks");
        }
        [HttpGet("GetFeedbackCount")]
        public IActionResult GetFeedbackCount(int id)
        {
            Console.WriteLine(id);
            var result = _feedbackRepository.GetFeedbackCount(id);
            if (result != "")
            {
                return Ok(result);
            }
            return BadRequest("Cannot get feedbacks");
        }
        

        private static List<string> standardPaths = new List<string>();
        private static List<string> standardPathsWithFile = new List<string>();
        private static int global_cwsid = 0;
        [HttpPost("read")]
        public IActionResult getFiles([FromForm] string? path)
        {
            Console.WriteLine("path ======================================= " + path + "======================================");

            List<Material.FileManagerItem> directories = new List<Material.FileManagerItem>();
            var conn = _materialService.connection;
            if (path == null)
            {

                // Example database query - assumes you have a method to execute the query and return results
                var standardCmd = new NpgsqlCommand("SELECT c_standard FROM t_standards", conn);//   ===============>                 CHANGE THIS QUERY
                using (var reader = standardCmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Material.FileManagerItem directory = new Material.FileManagerItem
                        {
                            Name = reader.GetString(0),
                            Size = 0L,
                            Path = reader.GetString(0),
                            Extension = "",
                            IsDirectory = true,
                            HasDirectories = true,
                            Created = DateTime.Now,
                            Modified = DateTime.Now

                        };
                        directories.Add(directory);
                        standardPaths.Add(directory.Path);
                    }
                }


            }
            else if (standardPaths.Contains(path))
            {
                Console.WriteLine("runnning");
                Console.WriteLine("=============================>>>>>>>>>>>>>>>>" + global_cwsid);
                var standardCmd = new NpgsqlCommand("select s.c_subject_name,c.c_id from t_subjects s join t_classwise_subjects c on s.c_subject_id = c.c_subject_id where c.c_standard = @std ", conn);
                standardCmd.Parameters.AddWithValue("std", path);
                using (var reader = standardCmd.ExecuteReader())
                {

                    while (reader.Read())
                    {
                        Material.FileManagerItem directory = new Material.FileManagerItem
                        {
                            Name = reader.GetString(0),
                            Size = 0L,
                            Path = $"{path}/{reader.GetString(0)}/{reader.GetInt32(1)}",
                            Extension = "",
                            IsDirectory = true,
                            HasDirectories = true,
                            Created = DateTime.Now,
                            Modified = DateTime.Now

                        };
                        directories.Add(directory);
                        standardPathsWithFile.Add(directory.Path);
                    }
                }
            }
            else if (standardPathsWithFile.Contains(path))
            {

                string[] pathArr = path.Split('/');
                var cwsid = Convert.ToInt32(pathArr[2]);
                var selectMaterialCmd = new NpgsqlCommand("SELECT c_materialid,c_materialname,c_uploadtime,c_file FROM t_material where c_cwsid = @cwsid ", conn);
                selectMaterialCmd.Parameters.AddWithValue("cwsid", cwsid);
                using (var reader = selectMaterialCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var cFile = reader.GetString(3);
                        var cDir = Directory.GetCurrentDirectory();
                        var vParth = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");


                        var cPath = vParth + cFile;

                        if (!string.IsNullOrEmpty(cPath) && System.IO.File.Exists(cPath))
                        {
                            FileInfo file = new FileInfo(cPath);
                            Material.FileManagerItem directory = new Material.FileManagerItem
                            {
                                Name = reader.GetString(1),
                                Size = file.Length,
                                Path = cFile,
                                Extension = file.Extension,
                                IsDirectory = false,
                                HasDirectories = false,
                                Created = file.CreationTime,
                                Modified = file.LastWriteTime

                            };
                            directories.Add(directory);
                        }
                    }
                }
            }

            return Ok(directories);
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(IFormFile file)
        {
            Console.WriteLine(global_cwsid);
            var userId = 144;
                        Console.WriteLine("===================>>>>>>>>>>>>>>>>>>>Hasi ACCess Can go FURTHER CWSID::::::::::::" + userId);

            if (file != null)
            {
                if (global_cwsid != 0)
                {

                    var tId = _materialService.GetTeacherId(userId);

                    if (_materialService.hasAccess(tId, global_cwsid))
                    {
                        Console.WriteLine("===================>>>>>>>>>>>>>>>>>>>Hasi ACCess Can go FURTHER CWSID::::::::::::" + global_cwsid);

                        var task = await _materialService.UploadMaterial(global_cwsid, file, tId);

                        if (task)
                        {
                            return Ok(new Material.UploadResponse
                            {
                                Success = true,
                                FileName = file.FileName
                            });
                        }
                        else { Console.WriteLine("Not Don =================>>>>>>>>>>>>>>>>"); }
                    }
                    else return Unauthorized(new { Success = "You don't have access to this directory" });
                }

            }

            return BadRequest(new Material.UploadResponse { Success = false });
        }
        [HttpPost("changePath")]
        public IActionResult changeCurrentPath([FromQuery] string? path)
        {

            path = path.Trim();
            if (path.Contains("/"))
            {
                string[] patharr = path.Split('/');
                if (int.TryParse(patharr[patharr.Length - 1], out int parseid))
                {
                    global_cwsid = parseid;
                    Console.WriteLine("=============================>>>>>>>>>>>>>>>>   Can go further " + global_cwsid);
                }
            }
            else { global_cwsid = 0; }
            Console.WriteLine("=============================>>>>>>>>>>>>>>>>" + global_cwsid);
            return Ok(new { path = path });
        }
        [HttpGet("download")]
        public IActionResult Download([FromQuery] string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return BadRequest("File path is required.");
            }
            var cDir = Directory.GetCurrentDirectory();
            var vParth = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            // Ensure the path is within the allowed root directory
            var filePath = vParth + path;
            Console.WriteLine(filePath);
            if (!filePath.StartsWith(vParth))
            {
                return Unauthorized("Access to this file path is not allowed.");
            }

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            // Determine the content type for the response
            var contentType = "application/octet-stream"; // Default binary type
            var fileExtension = Path.GetExtension(filePath);
            if (fileExtension == ".pdf") contentType = "application/pdf";

            // Read the file stream and return it as a FileResult
            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType, Path.GetFileName(filePath));
        }
        [HttpPost("deleteMaterial")]
        public IActionResult DeleteMaterial([FromForm] string? path, [FromForm] string? name, [FromQuery] string? n)
        {
            try
            {
                var tId = _materialService.GetTeacherId(144);
                var conn = _materialService.connection;
                if (_materialService.hasAccess(tId, global_cwsid))
                {
                    Console.WriteLine(">>>>>>>>>>>" + tId + "<<<<<<<<<<<<<" + global_cwsid + "<<<<<<<<<<<<<" + path);
                    if (_materialService.DeleteMaterial(path, tId, global_cwsid))
                    {
                        Console.WriteLine("Successfully deleted!!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("" + ex.Message);
            }

            return Ok(new { success = true });

        }


    }
}