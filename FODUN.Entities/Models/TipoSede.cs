
namespace FODUN.Entities.Models
{
    public class TipoSede
    {
        public int TipoSedeId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        // Navegación
        public ICollection<Sede> Sedes { get; set; } = new List<Sede>();
    }
}
