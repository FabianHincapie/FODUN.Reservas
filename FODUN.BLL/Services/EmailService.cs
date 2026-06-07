using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace FODUN.BLL.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCorreoRecuperacionAsync(string email, string nombre, string token)
        {
            var urlRecuperacion = $"https://localhost/Account/RestablecerPassword?token={token}";

            var cuerpo = $@"
                <h2>Recuperación de Contraseña - FODUN</h2>
                <p>Hola <strong>{nombre}</strong>,</p>
                <p>Recibimos una solicitud para restablecer tu contraseña.</p>
                <p>Haz clic en el siguiente enlace para restablecerla:</p>
                <a href='{urlRecuperacion}' 
                   style='background-color:#8B0000;color:white;padding:10px 20px;
                          text-decoration:none;border-radius:5px;'>
                   Restablecer Contraseña
                </a>
                <p>Este enlace expirará en <strong>2 horas</strong>.</p>
                <p>Si no solicitaste este cambio, ignora este correo.</p>
                <br/>
                <p>Sistema de Reservas FODUN</p>";

            await EnviarCorreoAsync(email, "Recuperación de Contraseña - FODUN", cuerpo);
        }

        public async Task EnviarConfirmacionReservaAsync(string email, string nombre, int reservaId)
        {
            var cuerpo = $@"
                <h2>Confirmación de Reserva - FODUN</h2>
                <p>Hola <strong>{nombre}</strong>,</p>
                <p>Tu reserva <strong>#{reservaId}</strong> ha sido registrada exitosamente.</p>
                <p>Puedes consultar los detalles de tu reserva ingresando al sistema.</p>
                <br/>
                <p>Sistema de Reservas FODUN</p>";

            await EnviarCorreoAsync(email, $"Confirmación Reserva #{reservaId} - FODUN", cuerpo);
        }

        private async Task EnviarCorreoAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
            var smtpUser = _configuration["EmailSettings:SmtpUser"];
            var smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            var mensaje = new MailMessage
            {
                From = new MailAddress(fromEmail!, fromName),
                Subject = asunto,
                Body = cuerpoHtml,
                IsBodyHtml = true
            };

            mensaje.To.Add(destinatario);

            await client.SendMailAsync(mensaje);
        }
    }
}