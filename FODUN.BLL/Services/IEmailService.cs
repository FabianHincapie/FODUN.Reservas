namespace FODUN.BLL.Services
{
    public interface IEmailService
    {
        Task EnviarCorreoRecuperacionAsync(string email, string nombre, string token);
        Task EnviarConfirmacionReservaAsync(string email, string nombre, int reservaId);
    }
}