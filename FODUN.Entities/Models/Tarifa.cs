namespace FODUN.Entities.Models
{
    public class Tarifa
    {
        public int TarifaId { get; set; }
        public int SedeId { get; set; }
        public int TemporadaId { get; set; }
        public int NumHabitaciones { get; set; } = 1;
        public int PersonasMin { get; set; } = 1;
        public int PersonasMax { get; set; }
        public decimal ValorNoche { get; set; }
        public decimal ValorPersonaAdicional { get; set; }
        public bool Activa { get; set; } = true;

        // Navegación
        public Sede Sede { get; set; } = null!;
        public Temporada Temporada { get; set; } = null!;
    }
}