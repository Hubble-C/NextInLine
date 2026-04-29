using NextInLine.Models.ViewModels;

namespace NextInLine.Interfaces;

// Definimos la interfaz que servira como contrato para el servicio de turnos
public interface ITurnService
{
   
    // Recibe los datos del cliente (Documento y Nombre)
    Task<bool> GenerateTurnAsync(UserTicketViewModel model);
    
    // Devuelve el turno actual, el asesor y la lista de siguientes
    Task<UserWaitingRoomViewModel> GetWaitingRoomDataAsync();

    // Metodo para que el administrador vea las estadisticas del dashboard
    Task<AdminDashboardViewModel> GetDashboardDataAsync();

    // Metodo para obtener el estado actual de la cola para el asesor
    Task<QueueControlViewModel> GetQueueControlDataAsync();
    
    // Metodo para llamar al siguiente turno disponible
    Task<bool> CallNextTurnAsync();

    // Metodo para cancelar un turno especifico usando su codigo unico
    Task<bool> CancelTurnAsync(string ticketCode);
}