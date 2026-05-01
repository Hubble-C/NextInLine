using Microsoft.EntityFrameworkCore;
using NextInLine.Data;
using NextInLine.Enums;
using NextInLine.Models;
using NextInLine.Response;

namespace NextInLine.Services;

public class TicketService
{
    private readonly MysqlDbContext _dbContext;
    private const string PREFIX = "TCK";
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

    public ServiceResponse<Ticket> AddTicket(int userId)
    {
        try
        {
            // 1. Validar si ya tiene ticket activo
            var ticketRepeat = _dbContext.tickets
                .FirstOrDefault(t => t.UserId == userId && t.Status == TicketStatus.open);

            if (ticketRepeat != null)
            {
                return new ServiceResponse<Ticket>()
                {
                    Success = false,
                    Message = "User already has an active ticket",
                    Data = ticketRepeat
                };
            }

            // 2. Obtener último ticket para generar código
            var lastTicket = _dbContext.tickets
                .OrderByDescending(t => t.Id)
                .FirstOrDefault();

            int nextNumber = 1;

            if (lastTicket != null && !string.IsNullOrEmpty(lastTicket.Code))
            {
                var parts = lastTicket.Code.Split('-');

                if (parts.Length == 2 && int.TryParse(parts[1], out int number))
                {
                    nextNumber = number + 1;
                }
            }

            string newCode = $"TCK-{nextNumber:D3}";

            // 3. Crear ticket
            var newTicket = new Ticket()
            {
                UserId = userId,
                Code = newCode,
                Status = TicketStatus.open,
                CreatedAt = DateTime.Now
            };

            _dbContext.tickets.Add(newTicket);
            _dbContext.SaveChanges();

            // 4. Respuesta OK
            return new ServiceResponse<Ticket>()
            {
                Success = true,
                Message = "Ticket created successfully",
                Data = newTicket
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<Ticket>()
            {
                Success = false,
                Message = e.Message
            };
        }
    }
}