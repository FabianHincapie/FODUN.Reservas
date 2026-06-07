using FODUN.DAL.Context;
using FODUN.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace FODUN.BLL.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly FodunContext _context;

        public UsuarioService(FodunContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> LoginAsync(string nroDocumento, string password)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NroDocumento == nroDocumento && u.Activo);

            if (usuario == null) return null;

            var passwordHash = HashPassword(password, usuario.PasswordSalt!);
            if (passwordHash != usuario.PasswordHash) return null;

            return usuario;
        }

        public async Task<Usuario> RegistrarAsync(Usuario usuario, string password)
        {
            // Generar salt y hash
            usuario.PasswordSalt = GenerarSalt();
            usuario.PasswordHash = HashPassword(password, usuario.PasswordSalt);
            usuario.FechaRegistro = DateTime.Now;
            usuario.Activo = true;

            // Hashear respuesta secreta
            if (!string.IsNullOrEmpty(usuario.RespuestaSecreta))
                usuario.RespuestaSecreta = HashSimple(usuario.RespuestaSecreta.ToLower().Trim());

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<bool> ExisteDocumentoAsync(string nroDocumento)
        {
            return await _context.Usuarios.AnyAsync(u => u.NroDocumento == nroDocumento);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email);
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Activo);
        }

        public async Task<bool> GenerarTokenRecuperacionAsync(string email)
        {
            var usuario = await GetByEmailAsync(email);
            if (usuario == null) return false;

            usuario.TokenRecuperacion = Guid.NewGuid().ToString();
            usuario.TokenExpiracion = DateTime.Now.AddHours(2);

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RestablecerPasswordAsync(string token, string nuevaPassword)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.TokenRecuperacion == token
                    && u.TokenExpiracion > DateTime.Now);

            if (usuario == null) return false;

            usuario.PasswordSalt = GenerarSalt();
            usuario.PasswordHash = HashPassword(nuevaPassword, usuario.PasswordSalt);
            usuario.TokenRecuperacion = null;
            usuario.TokenExpiracion = null;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Usuario?> GetByIdAsync(int usuarioId)
        {
            return await _context.Usuarios.FindAsync(usuarioId);
        }

        public async Task ActualizarAsync(int usuarioId, string nombreCompleto, DateTime? fechaNacimiento,
            string? celular, string email, string? departamento, string? municipio,
            string? barrio, string? direccion, string? telefono)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return;

            usuario.NombreCompleto = nombreCompleto;
            usuario.FechaNacimiento = fechaNacimiento;
            usuario.Celular = celular;
            usuario.Email = email;
            usuario.Departamento = departamento;
            usuario.Municipio = municipio;
            usuario.Barrio = barrio;
            usuario.DireccionResidencia = direccion;
            usuario.TelefonoResidencia = telefono;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        // Métodos privados de seguridad
        private string GenerarSalt()
        {
            var saltBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            using var sha256 = SHA256.Create();
            var combined = Encoding.UTF8.GetBytes(password + salt);
            var hash = sha256.ComputeHash(combined);
            return Convert.ToBase64String(hash);
        }

        private string HashSimple(string texto)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
            return Convert.ToBase64String(bytes);
        }
    }
}