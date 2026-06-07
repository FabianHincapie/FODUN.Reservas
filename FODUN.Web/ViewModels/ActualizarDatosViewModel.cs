using System.ComponentModel.DataAnnotations;

namespace FODUN.Web.ViewModels
{
    public class ActualizarDatosViewModel
    {
        public int UsuarioId { get; set; }

        [Display(Name = "Número de Documento")]
        public string NroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date)]
        public DateTime? FechaNacimiento { get; set; }

        [Display(Name = "Celular")]
        public string? Celular { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "Correo electrónico no válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Departamento")]
        public string? Departamento { get; set; }

        [Display(Name = "Municipio")]
        public string? Municipio { get; set; }

        [Display(Name = "Barrio")]
        public string? Barrio { get; set; }

        [Display(Name = "Dirección de Residencia")]
        public string? DireccionResidencia { get; set; }

        [Display(Name = "Teléfono de Residencia")]
        public string? TelefonoResidencia { get; set; }
    }
}