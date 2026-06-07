using FODUN.BLL.Services;
using FODUN.Entities.Models;
using FODUN.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FODUN.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IEmailService _emailService;

        public AccountController(IUsuarioService usuarioService, IEmailService emailService)
        {
            _usuarioService = usuarioService;
            _emailService = emailService;
        }

        // GET: Login
        public IActionResult Login(string? returnUrl = null)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") != null)
                return RedirectToAction("Index", "Sedes");

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _usuarioService.LoginAsync(model.NroDocumento, model.Password);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Documento o contraseña incorrectos.");
                return View(model);
            }

            // Guardar sesión
            HttpContext.Session.SetInt32("UsuarioId", usuario.UsuarioId);
            HttpContext.Session.SetString("NombreUsuario", usuario.NombreCompleto);
            HttpContext.Session.SetString("NroDocumento", usuario.NroDocumento);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Index", "Sedes");
        }

        // GET: Registro
        public IActionResult Registro()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") != null)
                return RedirectToAction("Index", "Sedes");

            return View(new RegistroViewModel());
        }

        // POST: Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _usuarioService.ExisteDocumentoAsync(model.NroDocumento))
            {
                ModelState.AddModelError("NroDocumento", "Ya existe un usuario con ese número de documento.");
                return View(model);
            }

            if (await _usuarioService.ExisteEmailAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Ya existe un usuario con ese correo electrónico.");
                return View(model);
            }

            var usuario = new Usuario
            {
                NroDocumento = model.NroDocumento,
                NombreCompleto = model.NombreCompleto,
                FechaNacimiento = model.FechaNacimiento,
                Celular = model.Celular,
                Email = model.Email,
                Departamento = model.Departamento,
                Municipio = model.Municipio,
                Barrio = model.Barrio,
                DireccionResidencia = model.DireccionResidencia,
                TelefonoResidencia = model.TelefonoResidencia,
                PreguntaSecreta = model.PreguntaSecreta,
                RespuestaSecreta = model.RespuestaSecreta,
                AutorizaCorreo = model.AutorizaCorreo,
                AutorizaCelular = model.AutorizaCelular
            };

            await _usuarioService.RegistrarAsync(usuario, model.Password);

            TempData["Mensaje"] = "Registro exitoso. Por favor inicia sesión.";
            return RedirectToAction("Login");
        }

        // GET: RecuperarPassword
        public IActionResult RecuperarPassword()
        {
            return View(new RecuperarPasswordViewModel());
        }

        // POST: RecuperarPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecuperarPassword(RecuperarPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = await _usuarioService.GetByEmailAsync(model.Email);

            if (usuario != null)
            {
                try
                {
                    await _usuarioService.GenerarTokenRecuperacionAsync(model.Email);
                    await _emailService.EnviarCorreoRecuperacionAsync(
                        model.Email, usuario.NombreCompleto, usuario.TokenRecuperacion!);
                    TempData["Mensaje"] = "Correo enviado exitosamente.";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al enviar correo: {ex.Message}";
                }
            }
            else
            {
                TempData["Mensaje"] = "Si el correo existe, recibirás las instrucciones en tu bandeja de entrada.";
            }

            return RedirectToAction("Login");
        }

        // GET: RestablecerPassword
        public IActionResult RestablecerPassword(string token)
        {
            return View(new RestablecerPasswordViewModel { Token = token });
        }

        // POST: RestablecerPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerPassword(RestablecerPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var resultado = await _usuarioService.RestablecerPasswordAsync(model.Token, model.NuevaPassword);

            if (!resultado)
            {
                ModelState.AddModelError("", "El enlace de recuperación es inválido o ha expirado.");
                return View(model);
            }

            TempData["Mensaje"] = "Contraseña restablecida exitosamente.";
            return RedirectToAction("Login");
        }
        // GET: ActualizarDatos
        public async Task<IActionResult> ActualizarDatos()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login");

            var usuario = await _usuarioService.GetByIdAsync(usuarioId.Value);
            if (usuario == null)
                return RedirectToAction("Login");

            var model = new ActualizarDatosViewModel
            {
                UsuarioId = usuario.UsuarioId,
                NroDocumento = usuario.NroDocumento,
                NombreCompleto = usuario.NombreCompleto,
                FechaNacimiento = usuario.FechaNacimiento,
                Celular = usuario.Celular,
                Email = usuario.Email,
                Departamento = usuario.Departamento,
                Municipio = usuario.Municipio,
                Barrio = usuario.Barrio,
                DireccionResidencia = usuario.DireccionResidencia,
                TelefonoResidencia = usuario.TelefonoResidencia
            };

            return View(model);
        }
        
        // POST: ActualizarDatos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarDatos(ActualizarDatosViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId == null)
                return RedirectToAction("Login");

            await _usuarioService.ActualizarAsync(
                usuarioId.Value, model.NombreCompleto, model.FechaNacimiento,
                model.Celular, model.Email, model.Departamento, model.Municipio,
                model.Barrio, model.DireccionResidencia, model.TelefonoResidencia);

            HttpContext.Session.SetString("NombreUsuario", model.NombreCompleto);
            TempData["Mensaje"] = "Datos actualizados exitosamente.";
            return RedirectToAction("ActualizarDatos");
        }
        // Cerrar sesión
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}