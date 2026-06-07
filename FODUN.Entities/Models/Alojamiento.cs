namespace FODUN.Entities.Models
{
    public class Alojamiento
    {
        public int AlojamientoId { get; set; }
        public int SedeId { get; set; }
        public string Numero { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public int NumHabitaciones { get; set; } = 1;
        public int CapacidadMax { get; set; }
        public bool TieneBano { get; set; } = true;
        public bool TieneCocineta { get; set; }
        public bool TieneTelevision { get; set; }
        public bool TieneNevera { get; set; }
        public bool TieneTerraza { get; set; }
        public bool TieneSalaEstar { get; set; }
        public bool TieneParqueadero { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public Sede Sede { get; set; } = null!;
        public ICollection<DetalleReserva> DetalleReservas { get; set; } = new List<DetalleReserva>();
    }
}
