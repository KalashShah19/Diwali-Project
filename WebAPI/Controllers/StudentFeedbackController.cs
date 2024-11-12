using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Repositories;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentFeedbackController : ControllerBase
    {

        private readonly FeedbackRepository _studentFeedback;

        // private readonly FeesRepository _feesRepository;

        // private readonly SchoolInfoRepository _schoolInfoRepository;

        // private readonly StandardRepository _standardRepository;



        public StudentFeedbackController(IConfiguration configuration)
        {
             _studentFeedback = new(configuration);
            //  _feesRepository = new(configuration);
            //  _schoolInfoRepository = new(configuration);
            //  _standardRepository = new(configuration);
        }

        [HttpPost]
        [Route("CreateStudentFeedback")]
        public IActionResult CreateStudentFeedback(Feedback.Post feedbackByStudent)
        {
            _studentFeedback.AddFeedback(feedbackByStudent);
            return Ok("Feedback submitted successfully!");
        }

        [HttpGet]
        [Route("GetTeacher")]
        public IActionResult GetTeacher()
        {
            var teachers = _studentFeedback.GetTeachers();
            return Ok(teachers);
        }

        [HttpPost]
        [Route("GetStdFeedback")]
        public IActionResult GetStdFeedback(int id)
        {
            var teachers = _studentFeedback.GetFeedbacksByStudent(id);
            return Ok(teachers);
        }

    }
}