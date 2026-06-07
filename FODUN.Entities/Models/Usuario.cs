namespace FODUN.Entities.Models
{
    public class Usuario
    {
        public int UsuarioId { get; set; }
        public string NroDocumento { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string? Celular { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Departamento { get; set; }
        public string? Municipio { get; set; }
        public string? Barrio { get; set; }
        public string? DireccionResidencia { get; set; }
        public string? TelefonoResidencia { get; set; }
        public string? PreguntaSecreta { get; set; }
        public string? RespuestaSecreta { get; set; }
        public bool AutorizaCorreo { get; set; } = true;
        public bool AutorizaCelular { get; set; } = true;
        public string PasswordHash { get; set; } = string.Empty;
        public string? PasswordSalt { get; set; }
        public string? TokenRecuperacion { get; set; }
        public DateTime? TokenExpiracion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
