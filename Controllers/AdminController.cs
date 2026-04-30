using Microsoft.AspNetCore.Mvc;
using NextInLine.Services;

namespace NextInLine.Controllers;

public class AdminController : Controller
{
    private readonly TicketService _ticketService;
    public AdminController(TicketService ticketService)
    {
        _ticketService = ticketService;
    }
    public IActionResult Index()
    {
        var tickets =  _ticketService.GetAllTickets().Data;
        return View(tickets);
    }
    public IActionResult Dashboard()
    {
        return View();
    }
}