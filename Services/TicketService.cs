using NextInLine.Data;
using NextInLine.Models;
using NextInLine.Response;

namespace NextInLine.Services;

public class TicketService
{
    private readonly MysqlDbContext _dbContext;

    public TicketService(MysqlDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ServiceResponse<IEnumerable<Ticket>> GetAllTickets()
    {
        var tickets = _dbContext.tickets.ToList();
        return new ServiceResponse<IEnumerable<Ticket>>()
        {
            Data = tickets,
            Success = true
        };
    }
    
    /*Find one ticket by their ID */
    public ServiceResponse<Ticket> GetTicketById(int ticketId)
    {
        var ticket = _dbContext.tickets.FirstOrDefault(t => t.Id == ticketId);
        if (ticket == null)
        {
            return new ServiceResponse<Ticket>()
            {
                Success = false,
                Message = "Ticket not found"
            };
        }
        return new ServiceResponse<Ticket>()
        {
            Data = ticket,
            Success = true
        };
    }
}