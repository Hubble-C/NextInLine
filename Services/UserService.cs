using Microsoft.EntityFrameworkCore;
using NextInLine.Data;
using NextInLine.Enums;
using NextInLine.Models;
using NextInLine.Response;
using NextInLine.Services;

namespace NextInLine.Services;

public class UserService
{
    private readonly MysqlDbContext _dbContext;
    private readonly TicketService _ticketService;
    private readonly PrinterService _printerService;

    public UserService(MysqlDbContext dbContext, TicketService ticketService, PrinterService printerService)
    {
        _dbContext = dbContext;
        _ticketService = ticketService;
        _printerService = printerService;
    }

    public async Task<ServiceResponse<User>> Create(User user)
    {
        try
        {
            // 1. Buscar si ya existe por email o documento
            var existingUser = await _dbContext.users
                .FirstOrDefaultAsync(u =>
                    u.Email == user.Email ||
                    u.Document == user.Document
                );

            // 2. Si ya existe → usar ese usuario
            if (existingUser != null)
            {
                var hasTicket = await _dbContext.tickets
                    .AnyAsync(t =>
                        t.UserId == existingUser.Id &&
                        t.Status == TicketStatus.open
                    );

                if (hasTicket)
                {
                    return new ServiceResponse<User>()
                    {
                        Success = false,
                        Message = "User already has an active ticket"
                    };
                }

                // ✅ usar existingUser.Id, NO user.Id
                var ticket = _ticketService.AddTicket(existingUser.Id);

                if (ticket.Success && ticket.Data != null)
                    _printerService.PrintTicket(ticket.Data.Code, existingUser.Name);

                return new ServiceResponse<User>()
                {
                    Success = true,
                    Message = $"Ticket created: {ticket.Data!.Code}",
                    Data = existingUser
                };
            }

            // 3. Si no existe → crear usuario
            _dbContext.users.Add(user);
            await _dbContext.SaveChangesAsync(); // ← aquí EF asigna user.Id

            // 4. Crear ticket
            var newTicket = _ticketService.AddTicket(user.Id);

            if (newTicket.Success && newTicket.Data != null)
                _printerService.PrintTicket(newTicket.Data.Code, user.Name);

            return new ServiceResponse<User>()
            {
                Success = true,
                Message = $"User and ticket created: {newTicket.Data!.Code}",
                Data = user
            };
        }
        catch (Exception e)
        {
            return new ServiceResponse<User>()
            {
                Success = false,
                Message = e.Message
            };
        }
    }
}