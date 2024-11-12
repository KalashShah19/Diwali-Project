using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers;

public class AdminController : Controller
{
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
}