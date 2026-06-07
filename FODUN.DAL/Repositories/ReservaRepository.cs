using FODUN.DAL.Context;
using FODUN.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace FODUN.DAL.Repositories
{
    public class ReservaRepository : Repository<Reserva>, IReservaRepository
    {
        private readonly FodunContext _context;

        public ReservaRepository(FodunContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Reserva>> GetReservasByUsuarioAsync(int usuarioId)
        {
            return await _context.Reservas
                .Include(r => r.Sede)
                .Include(r => r.Temporada)
                .Include(r => r.Usuario)
                .Include(r => r.DetalleReservas)
                    .ThenInclude(d => d.Alojamiento)
                .Where(r => r.UsuarioId == usuarioId)
                .OrderByDescending(r => r.FechaReserva)
                .ToListAsync();
        }

        public async Task<IEnumerable<Alojamiento>> GetDisponibilidadAsync(
            int sedeId, DateTime fechaInicio, DateTime fechaFin)
        {
            // Alojamientos ocupados en ese rango
            var ocupados = await _context.DetalleReservas
                .Where(d => d.Reserva.SedeId == sedeId
                    && d.Reserva.Estado != "Cancelada"
                    && d.Reserva.FechaLlegada < fechaFin
                    && d.Reserva.FechaSalida > fechaInicio)
                .Select(d => d.AlojamientoId)
                .ToListAsync();

            // Retornar los disponibles
            return await _context.Alojamientos
                .Where(a => a.SedeId == sedeId
                    && a.Activo
                    && !ocupados.Contains(a.AlojamientoId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Alojamiento>> GetDisponibilidadPersonasAsync(
            int sedeId, DateTime fechaInicio, DateTime fechaFin, int numPersonas)
        {
            var ocupados = await _context.DetalleReservas
                .Where(d => d.Reserva.SedeId == sedeId
                    && d.Reserva.Estado != "Cancelada"
                    && d.Reserva.FechaLlegada < fechaFin
                    && d.Reserva.FechaSalida > fechaInicio)
                .Select(d => d.AlojamientoId)
                .ToListAsync();

            return await _context.Alojamientos
                .Where(a => a.SedeId == sedeId
                    && a.Activo
                    && a.CapacidadMax >= numPersonas
                    && !ocupados.Contains(a.AlojamientoId))
                .ToListAsync();
        }
    }
}