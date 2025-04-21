using M09Practica.Model.Forms;
using M09Practica.Model.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace M09Practica.Pages.Account
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de página para el registro de administradores, accesible solo para usuarios con rol Admin.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Controla el acceso restringido a administradores.
    /// Envía los datos de registro a la API con autenticación mediante token.
    /// Muestra mensajes de éxito o error según el resultado del registro.
    /// </remarks>
    public class RegisterAdminModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public RegisterAdminFormModel RegisterData { get; set; }

        public string ErrorMessage { get; set; }

        public RegisterAdminModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición GET para verificar permisos de acceso.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <returns>Permite el acceso solo a administradores; de lo contrario, devuelve prohibición.</returns>
        public IActionResult OnGet()
        {
            // Verificar que solo los administradores pueden acceder a esta página
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Page();
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición POST para procesar el registro de un nuevo administrador.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <returns>Redirige a la página principal tras registro exitoso o recarga la página con errores.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // Verificar que solo los administradores pueden registrar otros administradores
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("GameAPI");

            // Obtener el token del usuario actual
            var token = HttpContext.Session.GetString("AuthToken");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var registerDto = new RegisterAdminRequestDto
            {
                Username = RegisterData.Username,
                Name = RegisterData.Name,         
                Surname = RegisterData.Surname,
                Email = RegisterData.Email,
                Password = RegisterData.Password
                // El AuthCode se maneja en el backend
            };

            var content = new StringContent(
                JsonSerializer.Serialize(registerDto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/Auth/register-admin", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Administrador registrado correctamente";
                return RedirectToPage("/Index");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error al registrar administrador: {responseContent}");
            return Page();
        }
    }
}
