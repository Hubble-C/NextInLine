using Microsoft.AspNetCore.Mvc;
using NextInLine.Interfaces;
using NextInLine.Models.ViewModels;

namespace NextInLine.Controllers;

// Controlador encargado de gestionar las vistas y acciones del cliente
public class UserController : Controller
{
    private readonly ITurnService _turnService;

    // Constructor para inyectar el servicio 
    public UserController(ITurnService turnService)
    {
        _turnService = turnService;
    }

    // Accion para mostrar el formulario de registro de turnos
    [HttpGet]
    public IActionResult Turn()
    {
        return View();
    }

    // Accion para procesar el registro del turno cuando el usuario hace clic en enviar
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Turn(UserTicketViewModel model)
    {
        // Revisamos que los datos del formulario sean validos
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Llamamos al servicio para generar el turno en la base de datos
        var success = await _turnService.GenerateTurnAsync(model);

        if (success)
        {
            // Si todo sale bien, lo mandamos a la sala de espera (Index)
            return RedirectToAction("Index");
        }

        // Si hay un error, volvemos a mostrar el formulario
        return View(model);
    }

    // Accion para mostrar la sala de espera con los datos reales
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        // Pedimos los datos al servicio para "pintar" la vista
        var data = await _turnService.GetWaitingRoomDataAsync();
        return View(data);
    }
}