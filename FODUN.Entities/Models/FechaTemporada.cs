namespace FODUN.Entities.Models
{
    public class FechaTemporada
    {
        public int FechaTemporadaId { get; set; }
        public int TemporadaId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int Anio { get; set; }

        // Navegación
        public Temporada Temporada { get; set; } = null!;
    }
}
