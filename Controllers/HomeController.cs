using Microsoft.AspNetCore.Mvc;

namespace Lab8.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    public IActionResult About(string? id)
    {
        ViewBag.Message = "Your application description page. You entered the ID " + id;
        return View();
    }
}
