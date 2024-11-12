using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly SchoolInfoRepository infoRepository;
    private readonly StandardRepository standardRepository;
    private readonly FeesRepository feesRepository;
    private readonly FeedbackRepository feedbackRepository;
    private readonly StudentRepository studentRepository;
    private readonly SubjectRepository subjectRepository;
    private readonly TeacherRepository teacherRepository;
    private readonly ClasswiseSubjectRepository classwiseSubjectRepository;

    public AdminController(IConfiguration configuration)
    {
        infoRepository = new(configuration);
        standardRepository = new(configuration);
        feesRepository = new(configuration);
        feedbackRepository = new(configuration);
        studentRepository = new(configuration);
        subjectRepository = new(configuration);
        teacherRepository = new(configuration);
        classwiseSubjectRepository = new(configuration);
    }

    // School Info Management
    [HttpGet("GetSchoolInfo")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSchoolInfo()
    {
        try
        {
            SchoolInfo.Get schoolInfo = infoRepository.GetSchoolInfo();
            return StatusCode(StatusCodes.Status200OK, schoolInfo);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPatch("UpdateSchoolInfo")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateSchoolInfo([FromForm] SchoolInfo.Post schoolInfo)
    {
        try
        {
            if (ModelState.IsValid)
            {
                infoRepository.UpdateSchoolInfo(schoolInfo);
                return StatusCode(StatusCodes.Status200OK, new { message = "School information updated successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form correctly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Standard Management
    [HttpPost("AddStandard")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddStandard([FromQuery] int standard)
    {
        try
        {
            if (ModelState.IsValid)
            {
                standardRepository.AddStandard(standard);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Standard {standard} added successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please provide a standard." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("GetStandards")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetStandards()
    {
        try
        {
            List<string> standards = standardRepository.GetStandards();
            return StatusCode(StatusCodes.Status200OK, standards);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpDelete("RemoveStandard")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RemoveStandard([FromQuery] int standard)
    {
        try
        {
            if (ModelState.IsValid)
            {
                standardRepository.RemoveStandard(standard);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Standard {standard} removed successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please provide a standard to delete." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Fee Structure Management
    [HttpGet("GetFeeStructure")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetFeeStructure()
    {
        try
        {
            List<FeesInfo.Get> feeStructure = feesRepository.GetFeeStructure();
            return StatusCode(StatusCodes.Status200OK, feeStructure);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("GetFeeStructureByBatchYear")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetFeeStructureByBatchYear(string batchyear)
    {
        try
        {
            List<FeesInfo.Get> feeStructure = feesRepository.GetFeeStructure(batchyear);
            return StatusCode(StatusCodes.Status200OK, feeStructure);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("AddFeeInfo")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddFeeInfo([FromForm] FeesInfo.Post feesInfo)
    {
        try
        {
            if (ModelState.IsValid)
            {
                feesRepository.AddFeeInfo(feesInfo);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Fees Info added successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPatch("UpdateFeeInfo")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateFeeInfo([FromForm] FeesInfo.Get feesInfo)
    {
        try
        {
            if (ModelState.IsValid)
            {
                Console.Write(feesInfo.FeesID);
                feesRepository.UpdateFeeInfo(feesInfo);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Fees Info updated successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpDelete("RemoveFeeInfo")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RemoveFeeInfo([FromQuery] int feeinfoid)
    {
        try
        {
            if (ModelState.IsValid)
            {
                feesRepository.RemoveFeeInfo(feeinfoid);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Fee info with id {feeinfoid} removed successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please provide an id to remove fees information." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Feedback Management
    [HttpGet("GetFeedbacks")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetFeedbacks()
    {
        try
        {
            List<Feedback.Get> feedbacks = feedbackRepository.GetFeedbacks();
            return StatusCode(StatusCodes.Status200OK, feedbacks);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Student Management
    [HttpGet("GetStudentDetails")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetStudentDetails()
    {
        try
        {
            List<Student.StudentDetails> studentDetails = studentRepository.GetStudentsList();
            return StatusCode(StatusCodes.Status200OK, studentDetails);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("GetAdmissionRequest")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetAdmissionRequests()
    {
        try
        {
            List<User.AdmitRequest> admissionRequests = studentRepository.GetAdmissionRequests();
            return StatusCode(StatusCodes.Status200OK, admissionRequests);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("AdmitStudent")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AdmitStudent([FromForm] Student.AdmitStudent student)
    {
        try
        {
            if (ModelState.IsValid)
            {
                studentRepository.AdmitStudent(student);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Student with {student.UserId} admitted sucessfully." });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPut("UpdateStudentDetails")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateStudentDetails([FromForm] Student.AdminUpdate studentDetails)
    {
        try
        {
            if (ModelState.IsValid)
            {
                studentRepository.UpdateStudentDetails(studentDetails);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Student details updated successfully." });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Subject Management
    [HttpGet("GetSubjects")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetSubjects()
    {
        try
        {
            List<Subject> subjects = subjectRepository.GetSubjects();
            return StatusCode(StatusCodes.Status200OK, subjects);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("AddSubject")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddSubject(string subjectname)
    {
        try
        {
            if (ModelState.IsValid)
            {
                subjectRepository.AddSubject(subjectname);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Subject {subjectname} added sucessfully." });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please provide a subject name." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpDelete("RemoveSubject")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RemoveSubject([FromQuery] int subjectid)
    {
        try
        {
            if (ModelState.IsValid)
            {
                subjectRepository.RemoveSubject(subjectid);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Subject with id {subjectid} removed successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please provide an id to remove the subject." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Teacher Management
    [HttpGet("GetTeacherDetails")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetTeacherDetails()
    {
        try
        {
            List<Teacher.TeacherDetails> teachers = teacherRepository.GetTeacherList();
            return StatusCode(StatusCodes.Status200OK, teachers);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("GetJobRequests")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetJobRequests()
    {
        try
        {
            List<User.AdmitRequest> jobRequests = teacherRepository.GetJobRequests();
            return StatusCode(StatusCodes.Status200OK, jobRequests);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("HireTeacher")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult HireTeacher([FromForm] Teacher.HireTeacher teacher)
    {
        try
        {
            if (ModelState.IsValid)
            {
                teacherRepository.HireTeacher(teacher);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Teacher with {teacher.UserId} hired sucessfully." });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPut("UpdateTeacherDetails")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateTeacherDetails([FromForm] Teacher.AdminUpdate teacherDetails)
    {
        try
        {
            if (ModelState.IsValid)
            {
                teacherRepository.UpdateTeacherDetails(teacherDetails);
                return StatusCode(StatusCodes.Status200OK, new { message = "Teacher details updated successfully." });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Classwise subject management
    [HttpGet("GetClasswiseSubjects")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetClasswiseSubjects()
    {
        try
        {
            List<ClasswiseSubjects.Get> classwiseSubjects = classwiseSubjectRepository.GetClasswiseSubjects();
            return StatusCode(StatusCodes.Status200OK, classwiseSubjects);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPost("AddClasswiseSubject")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult AddClasswiseSubject([FromForm] ClasswiseSubjects.Post details)
    {
        try
        {
            if (ModelState.IsValid)
            {
                classwiseSubjectRepository.AddClasswiseSubject(details);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Subject added sucessfully." });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpDelete("RemoveClasswiseSubject")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RemoveClasswiseSubject([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                classwiseSubjectRepository.RemoveClasswiseSubject(id);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Class wise subject with id {id} removed successfully." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please provide an id to remove the details." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpPut("UpdateClasswiseSubject")]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult UpdateClasswiseSubject([FromForm] ClasswiseSubjects.Default details)
    {
        try
        {
            if (ModelState.IsValid)
            {
                classwiseSubjectRepository.UpdateClasswiseSubject(details);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Class Subject details updated successfully." });
            }
            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please fill out the form properly." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    // Common 
    [HttpDelete("RejectCandidate")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult RejectCandidate([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                teacherRepository.RejectCandidate(id);
                return StatusCode(StatusCodes.Status200OK, new { message = $"Candidate with id {id} is rejected." });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { message = "Please provide an id." });
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }

    [HttpGet("GetTeacherswithStudents")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult GetTeacherswithStudents()
    {
        try
        {
            List<TeacherwithStudent> teacherwithStudents = teacherRepository.GetTeacherswithStudents();
            return StatusCode(StatusCodes.Status200OK, teacherwithStudents);
        }
        catch (Exception e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = e.Message });
        }
    }
}
