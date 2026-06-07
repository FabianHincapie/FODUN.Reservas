using FODUN.Entities.Models;

namespace FODUN.BLL.Services
{
    public interface IUsuarioService
    {
        Task<Usuario?> LoginAsync(string nroDocumento, string password);
        Task<Usuario> RegistrarAsync(Usuario usuario, string password);
        Task<bool> ExisteDocumentoAsync(string nroDocumento);
        Task<bool> ExisteEmailAsync(string email);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> GenerarTokenRecuperacionAsync(string email);
        Task<bool> RestablecerPasswordAsync(string token, string nuevaPassword);
        Task<Usuario?> GetByIdAsync(int usuarioId);
        Task ActualizarAsync(int usuarioId, string nombreCompleto, DateTime? fechaNacimiento,
            string? celular, string email, string? departamento, string? municipio,
            string? barrio, string? direccion, string? telefono);
    }
}