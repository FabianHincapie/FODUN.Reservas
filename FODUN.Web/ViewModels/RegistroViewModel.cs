using System.ComponentModel.DataAnnotations;

namespace FODUN.Web.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "El número de documento es obligatorio")]
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

        [Required(ErrorMessage = "El departamento es obligatorio")]
        [Display(Name = "Departamento")]
        public string Departamento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El municipio es obligatorio")]
        [Display(Name = "Municipio")]
        public string Municipio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El barrio es obligatorio")]
        [Display(Name = "Barrio")]
        public string Barrio { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [Display(Name = "Dirección de Residencia")]
        public string DireccionResidencia { get; set; } = string.Empty;

        [Display(Name = "Teléfono de Residencia")]
        public string? TelefonoResidencia { get; set; }

        [Display(Name = "Pregunta Secreta")]
        public string? PreguntaSecreta { get; set; }

        [Display(Name = "Respuesta Secreta")]
        public string? RespuestaSecreta { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "La contraseña debe tener mínimo 4 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirme la contraseña")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;

        [Display(Name = "Autoriza envío de información al correo")]
        public bool AutorizaCorreo { get; set; } = true;

        [Display(Name = "Autoriza envío de información al celular")]
        public bool AutorizaCelular { get; set; } = true;
    }
}