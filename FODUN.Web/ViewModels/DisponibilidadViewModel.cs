using FODUN.Entities.Models;

namespace FODUN.Web.ViewModels
{
    public class DisponibilidadViewModel
    {
        public Sede? Sede { get; set; }
        public DateTime FechaLlegada { get; set; }
        public DateTime FechaSalida { get; set; }
        public int NumPersonas { get; set; }
        public bool ServicioLavanderia { get; set; }
        public List<AlojamientoDisponibleViewModel> Alojamientos { get; set; } = new();
        public decimal ValorTotal { get; set; }
        public int NumHabitaciones { get; set; }
    }

    public class AlojamientoDisponibleViewModel
    {
        public Alojamiento? Alojamiento { get; set; }
        public bool Disponible { get; set; }
        public decimal TarifaOrdinaria { get; set; }
        public decimal TarifaEspecial { get; set; }
        public bool Seleccionado { get; set; }
    }
}