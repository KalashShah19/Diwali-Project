using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Models;
using WebAPI.Repositories;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly SchoolInfoRepository _schoolInfoRepository;
        private readonly StudentRepository _studentRepository;
        private readonly FeesRepository _feesRepository;
        private readonly FeedbackRepository _feedbackRepository;

        public StudentController(IConfiguration configuration)
        {
            _schoolInfoRepository = new SchoolInfoRepository(configuration);
            _studentRepository = new StudentRepository(configuration);
            _feesRepository = new FeesRepository(configuration);
            _feedbackRepository = new FeedbackRepository(configuration);
        }

        [HttpGet("getSession")]
        [Produces("application/json")]
        public IActionResult getSession()
        {
            string? user_id = HttpContext.Session.GetString("c_user_id");
            Console.WriteLine("in student controller session id = "+user_id);
            return Ok(new { user_id = user_id });
        }

        [HttpPut("UpdateStudentProfile/{userId}")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        public IActionResult UpdateStudentProfile(int userId, [FromForm] User.UpdateDetails profile)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid student profile data.");

            profile.UserId = userId;

            try
            {
                var isUpdated = _studentRepository.UpdateStudentProfile(profile);
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


        [HttpGet("GetProfileById/{userId}")]
        public ActionResult<User.UpdateDetails> GetProfileById(int userId)
        {
            try
            {
                var studentProfile = _studentRepository.GetUpdateProfileListById(userId).FirstOrDefault();

                if (studentProfile == null)
                {
                    return NotFound("Student not found.");
                }

                return Ok(studentProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetFeesDetails/{userId}")]
        [Produces("application/json")]
        public ActionResult<FeesInfo.FeesDetailsForStudent> GetFeesDetails(int userId)
        {
            try
            {
                var studentDetails = _feesRepository.GetFeesDetailsStudent(userId);

                if (studentDetails == null || !studentDetails.Any())
                {
                    return NotFound("Student Fees Details not found.");
                }

                return Ok(studentDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("PayFees")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public IActionResult PayFees([FromBody] FeesInfo.PaymentRequest paymentRequest)
        {
            if (paymentRequest == null || string.IsNullOrEmpty(paymentRequest.Status) || string.IsNullOrEmpty(paymentRequest.CurrentStandard))
            {
                return BadRequest("Status and CurrentStandard are required.");
            }

            bool feesPaid = _feesRepository.PayFees(paymentRequest);
            if (feesPaid)
            {
                return Ok(feesPaid);
            }
            else
            {
                return BadRequest("Error in paying fees");
            }
        }


        [HttpGet("GetSchoolInfo")]
        public ActionResult<SchoolInfo.Get> GetSchoolInfo()
        {
            try
            {
                var schoolInfo = _schoolInfoRepository.GetSchoolInfo();
                return Ok(schoolInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("CreateStudentFeedback")]
        public IActionResult CreateStudentFeedback(Feedback.Post feedbackByStudent)
        {
            _feedbackRepository.AddFeedback(feedbackByStudent);
            return Ok("Feedback submitted successfully!");
        }

        [HttpGet]
        [Route("GetTeacher")]
        public IActionResult GetTeacher()
        {
            var teachers = _feedbackRepository.GetTeachers();
            return Ok(teachers);
        }


        [HttpPost]
        [Route("GetStdFeedback")]
        public IActionResult GetStdFeedback(int id)
        {
            var teachers = _feedbackRepository.GetFeedbacksByStudent(id);
            return Ok(teachers);
        }


    }
}
