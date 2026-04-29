namespace NextInLine.Models.ViewModels;

// Modelo para mostrar la informacion en la pantalla de espera del cliente
public class UserWaitingRoomViewModel
{
    // Codigo unico del turno (ejemplo: A-001)
    public string CurrentTurnCode { get; set; } = "---";

    // Nombre del asesor que atendera el turno 
    public string AdvisorName { get; set; } = "---";

    // Nombre o numero del modulo/cabina 
    public string CabinName { get; set; } = "---";

    // Lista de los proximos turnos que seran llamados despues
    public List<string> UpcomingTurns { get; set; } = new();
}