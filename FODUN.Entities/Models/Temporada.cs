namespace FODUN.Entities.Models
{
    public class Temporada
    {
        public int TemporadaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activa { get; set; } = true;

        // Navegación
        public ICollection<FechaTemporada> FechasTemporada { get; set; } = new List<FechaTemporada>();
        public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}