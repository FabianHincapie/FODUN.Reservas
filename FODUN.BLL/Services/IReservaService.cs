using FODUN.Entities.Models;

namespace FODUN.BLL.Services
{
    public interface IReservaService
    {
        Task<IEnumerable<Sede>> GetSedesAsync();
        Task<Sede?> GetSedeByIdAsync(int sedeId);
        Task<IEnumerable<Alojamiento>> GetDisponibilidadAsync(int sedeId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Alojamiento>> GetDisponibilidadPersonasAsync(int sedeId, DateTime fechaInicio, DateTime fechaFin, int numPersonas);
        Task<decimal> CalcularTarifaAsync(int sedeId, DateTime fechaInicio, DateTime fechaFin, int numPersonas, int numHabitaciones, bool lavanderia);
        Task<Reserva> CrearReservaAsync(Reserva reserva, int alojamientoId);
        Task<IEnumerable<Reserva>> GetReservasByUsuarioAsync(int usuarioId);
        Task<Reserva> CancelarReservaAsync(int reservaId, int usuarioId);
    }
}