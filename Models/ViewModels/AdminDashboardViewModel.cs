namespace NextInLine.Models.ViewModels;

// Modelo para mostrar las estadisticas globales en el panel principal
public class AdminDashboardViewModel
{
    // Conteo total de usuarios registrados hoy (Tabla User)
    public int TotalUsersCount { get; set; }

    // Tiempo promedio que la gente espera antes de ser atendida
    public string AvgWaitTime { get; set; } = "0 min";
}