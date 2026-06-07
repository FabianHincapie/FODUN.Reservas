using FODUN.DAL.Context;
using FODUN.DAL.Repositories;
using FODUN.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FODUN.BLL.Services
{
    public class ReservaService : IReservaService
    {
        private readonly FodunContext _context;
        private readonly IReservaRepository _reservaRepository;

        public ReservaService(FodunContext context, IReservaRepository reservaRepository)
        {
            _context = context;
            _reservaRepository = reservaRepository;
        }

        public async Task<IEnumerable<Sede>> GetSedesAsync()
        {
            return await _context.Sedes
                .Include(s => s.TipoSede)
                .Where(s => s.Activa)
                .OrderBy(s => s.Nombre)
                .ToListAsync();
        }

        public async Task<Sede?> GetSedeByIdAsync(int sedeId)
        {
            return await _context.Sedes
                .Include(s => s.TipoSede)
                .Include(s => s.Alojamientos)
                .FirstOrDefaultAsync(s => s.SedeId == sedeId);
        }

        public async Task<IEnumerable<Alojamiento>> GetDisponibilidadAsync(
            int sedeId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _reservaRepository.GetDisponibilidadAsync(sedeId, fechaInicio, fechaFin);
        }

        public async Task<IEnumerable<Alojamiento>> GetDisponibilidadPersonasAsync(
            int sedeId, DateTime fechaInicio, DateTime fechaFin, int numPersonas)
        {
            return await _reservaRepository.GetDisponibilidadPersonasAsync(sedeId, fechaInicio, fechaFin, numPersonas);
        }

        public async Task<decimal> CalcularTarifaAsync(
            int sedeId, DateTime fechaInicio, DateTime fechaFin,
            int numPersonas, int numHabitaciones, bool lavanderia)
        {
            int numNoches = (fechaFin - fechaInicio).Days;
            if (numNoches <= 0) throw new Exception("La fecha de salida debe ser posterior a la de llegada.");

            // Determinar temporada
            var temporada = await DeterminarTemporadaAsync(fechaInicio);

            // Obtener tarifa - buscar exacta o la más cercana
            var tarifa = await _context.Tarifas
                .Where(t => t.SedeId == sedeId
                    && t.TemporadaId == temporada.TemporadaId
                    && t.Activa
                    && t.PersonasMax >= numPersonas)
                .OrderBy(t => Math.Abs(t.NumHabitaciones - numHabitaciones))
                .ThenBy(t => t.ValorNoche)
                .FirstOrDefaultAsync();

            // Si no encontró con filtro de personas, buscar sin ese filtro
            if (tarifa == null)
            {
                tarifa = await _context.Tarifas
                    .Where(t => t.SedeId == sedeId
                        && t.TemporadaId == temporada.TemporadaId
                        && t.Activa)
                    .OrderBy(t => Math.Abs(t.NumHabitaciones - numHabitaciones))
                    .FirstOrDefaultAsync();
            }

            // Si sigue sin encontrar tarifa buscar en temporada Baja como fallback
            if (tarifa == null)
            {
                var tempBaja = await _context.Temporadas
                    .FirstAsync(t => t.Nombre == "Baja");
                tarifa = await _context.Tarifas
                    .Where(t => t.SedeId == sedeId
                        && t.TemporadaId == tempBaja.TemporadaId
                        && t.Activa)
                    .OrderBy(t => Math.Abs(t.NumHabitaciones - numHabitaciones))
                    .FirstOrDefaultAsync();
            }

            if (tarifa == null) return 0;
            decimal total = tarifa.ValorNoche * numNoches;

            // Personas adicionales
            if (numPersonas > tarifa.PersonasMax)
                total += (numPersonas - tarifa.PersonasMax) * tarifa.ValorPersonaAdicional * numNoches;

            // Lavandería Santa Marta
            if (lavanderia)
            {
                var sede = await _context.Sedes.FindAsync(sedeId);
                if (sede?.Ciudad == "Santa Marta")
                    total += 18000;
            }

            return total;
        }

        public async Task<Reserva> CrearReservaAsync(Reserva reserva, int alojamientoId)
        {
            // Verificar disponibilidad
            var ocupado = await _context.DetalleReservas
                .AnyAsync(d => d.AlojamientoId == alojamientoId
                    && d.Reserva.Estado != "Cancelada"
                    && d.Reserva.FechaLlegada < reserva.FechaSalida
                    && d.Reserva.FechaSalida > reserva.FechaLlegada);

            if (ocupado) throw new Exception("El alojamiento no está disponible para las fechas seleccionadas.");

            // Calcular tarifa
            reserva.ValorTotal = await CalcularTarifaAsync(
                reserva.SedeId, reserva.FechaLlegada, reserva.FechaSalida,
                reserva.NumPersonas, reserva.NumHabitaciones, reserva.ServicioLavanderia);

            // Determinar temporada
            var temporada = await DeterminarTemporadaAsync(reserva.FechaLlegada);
            reserva.TemporadaId = temporada.TemporadaId;
            reserva.FechaReserva = DateTime.Now;
            reserva.Estado = "Pendiente";

            // Obtener valor noche
            var tarifa = await _context.Tarifas
                .Where(t => t.SedeId == reserva.SedeId
                    && t.TemporadaId == reserva.TemporadaId
                    && t.Activa)
                .OrderBy(t => Math.Abs(t.NumHabitaciones - reserva.NumHabitaciones))
                .FirstOrDefaultAsync();

            int numNoches = (reserva.FechaSalida - reserva.FechaLlegada).Days;

            // Guardar reserva
            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Guardar detalle
            var detalle = new DetalleReserva
            {
                ReservaId = reserva.ReservaId,
                AlojamientoId = alojamientoId,
                ValorNoche = tarifa?.ValorNoche ?? 0,
                NumNoches = numNoches,
                SubTotal = (tarifa?.ValorNoche ?? 0) * numNoches
            };

            _context.DetalleReservas.Add(detalle);
            await _context.SaveChangesAsync();

            return reserva;
        }

        public async Task<Reserva> CancelarReservaAsync(int reservaId, int usuarioId)
        {
            var reserva = await _context.Reservas
                .FirstOrDefaultAsync(r => r.ReservaId == reservaId
                    && r.UsuarioId == usuarioId);

            if (reserva == null)
                throw new Exception("Reserva no encontrada o no pertenece al usuario.");

            if (reserva.Estado == "Cancelada")
                throw new Exception("La reserva ya está cancelada.");

            if (reserva.FechaLlegada <= DateTime.Today)
                throw new Exception("No se puede cancelar una reserva con fecha de llegada pasada o igual a hoy.");

            reserva.Estado = "Cancelada";
            _context.Reservas.Update(reserva);
            await _context.SaveChangesAsync();

            return reserva;
        }

        public async Task<IEnumerable<Reserva>> GetReservasByUsuarioAsync(int usuarioId)
        {
            return await _reservaRepository.GetReservasByUsuarioAsync(usuarioId);
        }

        private async Task<Temporada> DeterminarTemporadaAsync(DateTime fecha)
        {
            // Verificar si es Alta temporada por rango de fechas
            var esAlta = await _context.FechasTemporada
                .AnyAsync(ft => fecha >= ft.FechaInicio && fecha <= ft.FechaFin
                    && ft.Temporada.Nombre == "Alta");

            if (esAlta)
                return await _context.Temporadas.FirstAsync(t => t.Nombre == "Alta");

            // Verificar si es Especial (Lunes a Jueves)
            if (fecha.DayOfWeek >= DayOfWeek.Monday && fecha.DayOfWeek <= DayOfWeek.Thursday)
                return await _context.Temporadas.FirstAsync(t => t.Nombre == "Especial");

            return await _context.Temporadas.FirstAsync(t => t.Nombre == "Baja");
        }
    }
}