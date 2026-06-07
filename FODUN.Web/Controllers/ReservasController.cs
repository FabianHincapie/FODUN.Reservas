using FODUN.BLL.Services;
using FODUN.DAL.Context;
using FODUN.Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FODUN.Web.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IReservaService _reservaService;
        private readonly IEmailService _emailService;
        private readonly FodunContext _context;

        public ReservasController(IReservaService reservaService,
            IEmailService emailService, FodunContext context)
        {
            _reservaService = reservaService;
            _emailService = emailService;
            _context = context;
        }

        // POST: Crear reserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(int sedeId, DateTime fechaLlegada, DateTime fechaSalida,
            int numPersonas, int alojamientoId, int numHabitaciones)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var reserva = new Reserva
                {
                    UsuarioId = usuarioId.Value,
                    SedeId = sedeId,
                    FechaLlegada = fechaLlegada,
                    FechaSalida = fechaSalida,
                    NumPersonas = numPersonas,
                    NumHabitaciones = numHabitaciones
                };

                var nuevaReserva = await _reservaService.CrearReservaAsync(reserva, alojamientoId);

                TempData["Mensaje"] = $"Reserva #{nuevaReserva.ReservaId} creada exitosamente.";
                return RedirectToAction("MisReservas");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Detalle", "Sedes", new { id = sedeId });
            }
        }

        // POST: Cancelar reserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int reservaId)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var reserva = await _reservaService.CancelarReservaAsync(reservaId, usuarioId.Value);
                TempData["Mensaje"] = $"Reserva #{reservaId} cancelada exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction("MisReservas");
        }

        // GET: Mis Reservas
        public async Task<IActionResult> MisReservas()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            var reservas = await _reservaService.GetReservasByUsuarioAsync(usuarioId.Value);
            return View(reservas);
        }

        // GET: Enviar comprobante por correo
        public async Task<IActionResult> EnviarComprobante(int id)
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var reservas = await _reservaService.GetReservasByUsuarioAsync(usuarioId.Value);
                var reserva = reservas.FirstOrDefault(r => r.ReservaId == id);

                if (reserva == null)
                {
                    TempData["Error"] = "Reserva no encontrada.";
                    return RedirectToAction("MisReservas");
                }

                await _emailService.EnviarConfirmacionReservaAsync(
                    reserva.Usuario?.Email ?? "",
                    reserva.Usuario?.NombreCompleto ?? "",
                    reserva.ReservaId);

                reserva.ComprobanteEnviado = true;
                _context.Reservas.Update(reserva);
                await _context.SaveChangesAsync();

                TempData["Mensaje"] = $"Comprobante de la reserva #{id} enviado a su correo exitosamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al enviar comprobante: {ex.Message}";
            }

            return RedirectToAction("MisReservas");
        }
    }
}