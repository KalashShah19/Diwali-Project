using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Libraries;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        private static string CaptchaValue = "1234";
        private readonly LoginRepository loginRepository;
        private readonly RegisterRepository registerRepository;
        private readonly CaptchaRepository captchaRepository;

        public LoginController(IConfiguration configuration)
        {
            loginRepository = new(configuration);
            registerRepository = new(configuration);
            captchaRepository = new(configuration);
        }

        [HttpPost("GetCaptcha")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public JsonResult GetCaptcha()
        {
            var (captchaModel, catpchatext) = captchaRepository.GetCaptchaModel();
            return new JsonResult(
                new
                {
                    CaptchaID = captchaModel.CaptchaID,
                    Captcha = captchaModel.Captcha,
                    catpchatext = catpchatext,
                }
            );
        }

        [HttpPost("Login")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ValidateUser([FromBody] Login login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    (int? c_user_id, string c_role) = loginRepository.ValidateUser(login);
                    Console.WriteLine(c_user_id.ToString());

                    if (c_user_id.HasValue)
                    {
                        HttpContext.Session.SetString("c_user_id",c_user_id.ToString());
                        Console.WriteLine("session id = "+ HttpContext.Session.GetString("c_user_id"));
                        Console.WriteLine("inside");
                        return StatusCode(
                            StatusCodes.Status200OK,
                            new { message = "Login successful", c_user_id = c_user_id, c_role = c_role }
                        );
                    }
                    else
                    {
                        return StatusCode(
                            StatusCodes.Status401Unauthorized,
                            new { message = "Invalid email or password" }
                        );
                    }
                }

                return StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { message = "Please fill out the form properly." }
                );
            }
            catch (Exception e)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = e.Message }
                );
            }
        }

        [HttpPost("RequestPasswordChange")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult RequestPasswordChange([FromBody] string email)
        {
            Console.WriteLine(email);
            try
            {
                if (ModelState.IsValid && email != null)
                {
                    (string status, string generated_otp) = loginRepository.RequestPasswordChange(
                        email
                    );
                    Console.WriteLine("otp = " + generated_otp);
                    if (status == "true")
                    {
                        return Ok(
                            new
                            {
                                success = true,
                                message = "OTP sent. Check your email to reset your password.",
                                otp = generated_otp,
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(
                            StatusCodes.Status500InternalServerError,
                            new { success = false, message = status }
                        );
                    }
                }

                return BadRequest(
                    new { success = false, message = "Please enter a valid email address." }
                );
            }
            catch (Exception e)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { success = false, message = e.Message }
                );
            }
        }

        [HttpPost("Register")]
        [Produces("application/json")]
        [Consumes("application/json", "multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromForm] User.Register registerModel)
        {
            Console.WriteLine("registerModel?.ProfileImage");
            Console.WriteLine(registerModel?.ProfileImage);
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new { message = "Please fill out the form properly." }
                    );
                }

                // Call repository method to register the user
                bool isRegistered = await registerRepository.RegisterUser(registerModel);

                if (isRegistered)
                {
                    return StatusCode(
                        StatusCodes.Status200OK,
                        new { message = "Registration successful" }
                    );
                }
                else
                {
                    return StatusCode(
                        StatusCodes.Status500InternalServerError,
                        new { message = "Registration failed. Please try again." }
                    );
                }
            }
            catch (Exception e)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { message = $"Internal server error: {e.Message}" }
                );
            }
        }

        [HttpPost("Newpassword")]
        [Produces("application/json")]
        [Consumes("application/json", "multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Newpassword([FromBody] Forgotpassword Forgotpassword)
        {
            Console.WriteLine(Forgotpassword);
            try
            {
                if (ModelState.IsValid)
                {
                    string? result = loginRepository.Newpassword(Forgotpassword);

                    if (result == "Password updated successfully.")
                    {
                        return Ok(result);
                    }
                    else if (result == "Email not found.")
                    {
                        return NotFound(result);
                    }
                }

                return BadRequest(
                    new { success = false, message = "Please enter a valid email address." }
                );
            }
            catch (Exception e)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    new { success = true, message = e.Message }
                );
            }
        }

        [HttpPost("AddStudentRole")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddStudentRole(
            [FromBody] Student.RoleStudentRequest request
        )
        {
            if (
                request == null
                || string.IsNullOrEmpty(request.Email)
                || string.IsNullOrEmpty(request.Standard)
            )
            {
                return BadRequest("Email and Standard are required.");
            }

            var result = await registerRepository.RoleStudent(request.Email, request.Standard); // Updated call

            if (result)
            {
                return Ok("Student role added successfully.");
            }
            else
            {
                return NotFound("Email not found in t_users or an error occurred.");
            }
        }

        [HttpPost("AddTeacherRole")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddTeacherRole(
            [FromBody] Teacher.RoleTeacherRequest request
        )
        {
            if (
                request == null
                || string.IsNullOrEmpty(request.Email)
                || string.IsNullOrEmpty(request.Standard)
                || string.IsNullOrEmpty(request.Qualification)
            )
            {
                return BadRequest("Email, Standard, and Qualification are required.");
            }

            var result = await registerRepository.RoleTeacher(
                request.Email,
                request.Standard,
                request.Qualification
            );

            if (result)
            {
                return Ok("Teacher role added successfully.");
            }
            else
            {
                return NotFound("Email not found in t_users or an error occurred.");
            }
        }
    }
}
