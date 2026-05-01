using Microsoft.AspNetCore.Mvc;
using NextInLine.Interfaces;
using NextInLine.Models;
using NextInLine.Models.ViewModels;
using NextInLine.Services;

namespace NextInLine.Controllers;

// Controlador encargado de gestionar las vistas y acciones del cliente
public class UserController : Controller
{
    private readonly ITurnService _turnService;
    private readonly UserService _userService;
    // Constructor para inyectar el servicio 
    public UserController(ITurnService turnService,  UserService userService)
    {
        _turnService = turnService;
        _userService = userService;
    }

    // Accion para mostrar el formulario de registro de turnos
    public IActionResult Turn()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(User user)
    {
        var result = await _userService.Create(user);

        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            return View("Turn");
        }

        // Puedes pasar el código del ticket
        TempData["Success"] = result.Message;

        return RedirectToAction("Turn");
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