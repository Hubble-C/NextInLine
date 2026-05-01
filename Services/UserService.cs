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

    public UserService(MysqlDbContext dbContext,  TicketService ticketService)
    {
        _dbContext = dbContext;
        _ticketService = ticketService;
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
                // Validar ticket activo
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

                // Crear ticket
                var ticket =  _ticketService.AddTicket(user.Id);

                return new ServiceResponse<User>()
                {
                    Success = true,
                    Message = $"Ticket created: {ticket.Data.Code}",
                    Data = existingUser
                };
            }

            // 3. Si no existe → crear usuario
            _dbContext.users.Add(user);
            await _dbContext.SaveChangesAsync();

            // 4. Crear ticket
            var newTicket =  _ticketService.AddTicket(user.Id);

            return new ServiceResponse<User>()
            {
                Success = true,
                Message = $"User and ticket created: {newTicket.Data.Code}",
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