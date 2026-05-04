using Microsoft.AspNetCore.SignalR;

namespace NextInLine.TurnsHub;

public class TurnHub : Hub
{
    public async Task NotifyTurnCalled(string code)
    {
        await Clients.All.SendAsync("ReceiveTurn", code);
    }
}