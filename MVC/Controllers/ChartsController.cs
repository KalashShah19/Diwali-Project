using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public class ChartsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
