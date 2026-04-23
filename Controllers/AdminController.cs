using Microsoft.AspNetCore.Mvc;

namespace SistemaDeTurnos.Controllers;

public class AdminController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Dashboard()
    {
        return View();
    }
}