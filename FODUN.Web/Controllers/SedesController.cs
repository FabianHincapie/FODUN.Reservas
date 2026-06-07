using FODUN.BLL.Services;
using FODUN.DAL.Context;
using FODUN.Entities.Models;
using FODUN.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FODUN.Web.Controllers
{
    public class SedesController : Controller
    {
        private readonly IReservaService _reservaService;
        private readonly FodunContext _context;

        public SedesController(IReservaService reservaService, FodunContext context)
        {
            _reservaService = reservaService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
                return RedirectToAction("Login", "Account");

            var sedes = await _reservaService.GetSedesAsync();
            return View(sedes);
        }

        public async Task<IActionResult> Detalle(int id, DateTime? fechaLlegada,
     DateTime? fechaSalida, int ? numPersonas = null)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
                return RedirectToAction("Login", "Account");

            var sede = await _reservaService.GetSedeByIdAsync(id);
            if (sede == null) return NotFound();

            var model = new DisponibilidadViewModel
            {
                Sede = sede,
                NumPersonas = numPersonas ?? 0
            };

            // Solo buscar disponibilidad si el usuario seleccionó fechas
            if (fechaLlegada.HasValue && fechaSalida.HasValue
                && fechaSalida.Value > fechaLlegada.Value)
            {
                model.FechaLlegada = fechaLlegada.Value;
                model.FechaSalida = fechaSalida.Value;

                var alojamientosDisponibles = await _reservaService
                    .GetDisponibilidadAsync(id, fechaLlegada.Value, fechaSalida.Value);

                var todosAlojamientos = await _context.Alojamientos
                    .Where(a => a.SedeId == id && a.Activo)
                    .ToListAsync();

                foreach (var aloj in todosAlojamientos)
                {
                    var disponible = alojamientosDisponibles
                        .Any(a => a.AlojamientoId == aloj.AlojamientoId);

                    decimal tarifaOrdinaria = await _reservaService.CalcularTarifaAsync(
                        id, fechaLlegada.Value, fechaSalida.Value,
                        numPersonas.GetValueOrDefault(1), aloj.NumHabitaciones, false);

                    decimal tarifaEspecial = await _reservaService.CalcularTarifaAsync(
                        id, fechaLlegada.Value.AddDays(1), fechaSalida.Value.AddDays(1),
                        numPersonas.GetValueOrDefault(1), aloj.NumHabitaciones, false);

                    model.Alojamientos.Add(new AlojamientoDisponibleViewModel
                    {
                        Alojamiento = aloj,
                        Disponible = disponible,
                        TarifaOrdinaria = tarifaOrdinaria,
                        TarifaEspecial = tarifaEspecial
                    });
                }
            }

            return View(model);
        }
    }
}