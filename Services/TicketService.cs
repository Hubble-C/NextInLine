using NextInLine.Data;
using NextInLine.Enums;
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
    
    public ServiceResponse<IEnumerable<Ticket>> GetTicketsPendings()
    {
        var tickets = _dbContext.tickets.Where(t => t.Status == TicketStatus.pending).ToList();
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

    public ServiceResponse<Ticket> UpdateTicket(Ticket ticket)
    {
        var response = new ServiceResponse<Ticket>();
        try
        {
            var existingTicket = _dbContext.tickets.FirstOrDefault(t => t.Id == ticket.Id);

            if (existingTicket == null)
            {
                response.Success = false;
                response.Message = "Ticket no encontrado";
                return response;
            }

            // Actualizar campos (ajusta según tu modelo)
            existingTicket.Code = ticket.Code;
            existingTicket.Status = ticket.Status;

            _dbContext.SaveChanges();

            response.Data = existingTicket;
            response.Success = true;
            response.Message = "Ticket actualizado correctamente";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el ticket";
            // opcional: log ex
        }

        return response;
    }
}