using NextInLine.Interfaces;
using NextInLine.Models.ViewModels;

namespace NextInLine.Services; 

public class TurnServiceImplementation : ITurnService
{
    // Implementacion de los metodos (comentarios en espanol para repasar)
    
    public async Task<bool> GenerateTurnAsync(UserTicketViewModel model) => true;

    public async Task<UserWaitingRoomViewModel> GetWaitingRoomDataAsync()
    {
        return new UserWaitingRoomViewModel
        {
            CurrentTurnCode = "A-105",
            AdvisorName = "Sergio Alejandro",
            CabinName = "Modulo 4",
            UpcomingTurns = new List<string> { "A-106", "B-201" }
        };
    }

    public async Task<AdminDashboardViewModel> GetDashboardDataAsync() => new() { TotalUsersCount = 127 };

    public async Task<QueueControlViewModel> GetQueueControlDataAsync() => new() { CurrentTurnCode = "A-105" };

    public async Task<bool> CallNextTurnAsync() => true;
    
    public async Task<bool> CancelTurnAsync(string ticketCode) => true;
}