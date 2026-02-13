using Microsoft.AspNetCore.Mvc;

namespace WebDevProject.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return this.View();
    }
}
