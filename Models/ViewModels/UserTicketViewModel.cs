namespace NextInLine.Models.ViewModels;

// Clase para transportar los datos desde el formulario de registro al controlador
public class UserTicketViewModel
{
    // El numero de documento del usuario 
    public string Document { get; set; } = string.Empty;

    // El nombre completo del usuario para el registro
    public string Name { get; set; } = string.Empty;
}