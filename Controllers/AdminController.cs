using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NextInLine.Enums;
using NextInLine.Models.ViewModels;
using NextInLine.Services;
using NextInLine.TurnsHub;

namespace SistemaDeTurnos.Controllers;

public class AdminController : Controller
{
    private readonly TicketService _ticketService;
    private readonly UserService   _userService;
    private readonly IHubContext<TurnHub> _hub;

    public AdminController(TicketService ticketService, UserService userService, IHubContext<TurnHub> hub)
    {
        _ticketService = ticketService;
        _userService   = userService;
        _hub           = hub;
    }

    public IActionResult GetTicketsPendings()
    {
        var data = _ticketService.GetTicketsPendings().Data;
        return Json(data);
    }

    [HttpPost]
    public async Task<IActionResult> NextTurn()
    {
        var tickets   = _ticketService.GetAllTickets().Data;
        var actuallyT = tickets.FirstOrDefault(t => t.Status == TicketStatus.open);

        var next = tickets
            .Where(t => t.Status == TicketStatus.pending)
            .OrderBy(t => t.CreatedAt)
            .FirstOrDefault();

        if (actuallyT != null)
        {
            actuallyT.Status = TicketStatus.closed;
            _ticketService.UpdateTicket(actuallyT);
        }

        if (next == null)
        {
            await _hub.Clients.All.SendAsync("ReceiveTurn", "NO_TICKETS");
            return Ok("NO_TICKETS");
        }

        next.Status = TicketStatus.open;
        _ticketService.UpdateTicket(next);
        await _hub.Clients.All.SendAsync("ReceiveTurn", next.Code);
        return Ok(next.Code);
    }

    public IActionResult Index()
    {
        var tickets = _ticketService.GetAllTickets().Data;
        return View(tickets);
    }

    public IActionResult Dashboard()
    {
        var tickets = _ticketService.GetAllTickets().Data?.ToList() ?? new();
        var today   = DateTime.UtcNow.Date;

        var todayTickets   = tickets.Where(t => t.CreatedAt.Date == today).ToList();
        var pendingToday   = todayTickets.Where(t => t.Status == TicketStatus.pending).ToList();
        var completedToday = todayTickets.Count(t => t.Status == TicketStatus.closed);

        var waitMins = pendingToday
            .Select(t => (DateTime.UtcNow - t.CreatedAt).TotalMinutes)
            .ToList();

        var avgMin = waitMins.Any() ? waitMins.Average() : 0;
        var avgTs  = TimeSpan.FromMinutes(avgMin);
        var avgStr = avgTs.TotalMinutes < 1
            ? "< 1 min"
            : $"{(int)avgTs.TotalMinutes} min";

        var vm = new AdminDashboardViewModel
        {
            TotalUsersCount = tickets.Select(t => t.UserId).Distinct().Count(),
            TicketsToday    = todayTickets.Count,
            CompletedToday  = completedToday,
            AvgWaitTime     = avgStr
        };

        return View(vm);
    }
}
