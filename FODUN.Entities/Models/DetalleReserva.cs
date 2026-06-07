namespace FODUN.Entities.Models
{
    public class DetalleReserva
    {
        public int DetalleId { get; set; }
        public int ReservaId { get; set; }
        public int AlojamientoId { get; set; }
        public decimal ValorNoche { get; set; }
        public int NumNoches { get; set; }
        public decimal SubTotal { get; set; }

        // Navegación
        public Reserva Reserva { get; set; } = null!;
        public Alojamiento Alojamiento { get; set; } = null!;
    }
}
