using Microsoft.AspNetCore.Mvc;

namespace NextInLine.Controllers;

public class UserController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Turn()
    {
        return View();
    }
}