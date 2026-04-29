namespace NextInLine.Models.ViewModels;

// Modelo para que el asesor gestione la fila de turnos
public class QueueControlViewModel
{
    // Codigo del turno que se esta atendiendo actualmente
    public string CurrentTurnCode { get; set; } = "Ninguno";

    // Cantidad de personas que todavia estan en estado 'Pending' o 'Waiting'
    public int PendingTurnsCount { get; set; }
}