using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NextInLine.Enums;
using NextInLine.Services;
using NextInLine.TurnsHub;

namespace NextInLine.Controllers;

public class AdminController : Controller
{
    private readonly TicketService _ticketService;
    private readonly IHubContext<TurnHub> _hub;
    
    public AdminController(TicketService ticketService, IHubContext<TurnHub> hub)
    {
        _ticketService = ticketService;
        _hub = hub;
    }
    
    public IActionResult GetTicketsPendings()
    {
        var data = _ticketService.GetTicketsPendings().Data;
        return Json(data);
    }
    
    
    // 🔥This is to make next turn and show
    [HttpPost]
    public async Task<IActionResult> NextTurn()
    {
        var tickets = _ticketService.GetAllTickets().Data;
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
        else
        {
            next.Status = TicketStatus.open;
            _ticketService.UpdateTicket(next);
            await _hub.Clients.All.SendAsync("ReceiveTurn", next.Code);
            return Ok(next.Code);
        }
        
        return Ok("No there's next turn");
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