namespace FODUN.Entities.Models
{
    public class Reserva
    {
        public int ReservaId { get; set; }
        public int UsuarioId { get; set; }
        public int SedeId { get; set; }
        public DateTime FechaReserva { get; set; } = DateTime.Now;
        public DateTime FechaLlegada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int NumPersonas { get; set; }
        public int NumHabitaciones { get; set; } = 1;
        public bool ServicioLavanderia { get; set; }
        public bool ComprobanteEnviado { get; set; } = false;
        public int TemporadaId { get; set; }
        public decimal ValorTotal { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string? Observaciones { get; set; }

        // Navegación
        public Usuario Usuario { get; set; } = null!;
        public Sede Sede { get; set; } = null!;
        public Temporada Temporada { get; set; } = null!;
        public ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();
    }
}
