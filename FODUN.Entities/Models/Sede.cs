namespace FODUN.Entities.Models
{
    public class Sede
    {
        public int SedeId { get; set; }
        public int TipoSedeId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NombreCorto { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string Departamento { get; set; } = string.Empty;
        public string? Direccion { get; set; }
        public string? Descripcion { get; set; }
        public int CapacidadTotal { get; set; }
        public string? ImagenPrincipal { get; set; }
        public bool Activa { get; set; } = true;

        // Navegación
        public TipoSede TipoSede { get; set; } = null!;
        public ICollection<Alojamiento> Alojamientos { get; set; } = new List<Alojamiento>();
        public ICollection<Tarifa> Tarifas { get; set; } = new List<Tarifa>();
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}