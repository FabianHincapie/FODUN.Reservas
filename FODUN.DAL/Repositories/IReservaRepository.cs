using FODUN.Entities.Models;

namespace FODUN.DAL.Repositories
{
    public interface IReservaRepository : IRepository<Reserva>
    {
        Task<IEnumerable<Reserva>> GetReservasByUsuarioAsync(int usuarioId);
        Task<IEnumerable<Alojamiento>> GetDisponibilidadAsync(int sedeId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Alojamiento>> GetDisponibilidadPersonasAsync(int sedeId, DateTime fechaInicio, DateTime fechaFin, int numPersonas);
    }
}