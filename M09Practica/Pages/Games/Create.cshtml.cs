using M09Practica.Model.Forms;
using M09Practica.Model.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace M09Practica.Pages.Games
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de página para la creación de nuevos juegos, accesible solo para usuarios con rol Admin.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Controla el acceso restringido a administradores.
    /// Envía los datos del nuevo juego a la API con autenticación mediante token.
    /// Muestra mensajes de éxito o error según el resultado de la creación.
    /// </remarks>
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public GameFormModel GameData { get; set; }

        public string ErrorMessage { get; set; }

        public CreateModel(IHttpClientFactory clientFactory)
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
        /// Método que se ejecuta en la petición POST para procesar la creación de un nuevo juego.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <returns>Redirige a la página principal tras creación exitosa o recarga la página con errores.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // Verificar que solo los administradores pueden crear juegos
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

            var createGameDto = new CreateGameDto
            {
                Name = GameData.Name,
                Description = GameData.Description,
                ProfileImage = GameData.ProfileImage
            };

            var content = new StringContent(
                JsonSerializer.Serialize(createGameDto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/Game", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Juego creado correctamente";
                return RedirectToPage("/Index");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error al crear el juego: {responseContent}");
            return Page();
        }
    }
}
