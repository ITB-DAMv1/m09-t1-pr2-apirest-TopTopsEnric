using M09Practica.Model.Forms;
using M09Practica.Model.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace M09Practica.Pages.Games
{
    /// ��<summary>_PLACEHOLDER��
    /// Modelo de p�gina para la creaci�n de nuevos juegos, accesible solo para usuarios con rol Admin.
    /// ��</summary>_PLACEHOLDER��
    /// <remarks>
    /// Controla el acceso restringido a administradores.
    /// Env�a los datos del nuevo juego a la API con autenticaci�n mediante token.
    /// Muestra mensajes de �xito o error seg�n el resultado de la creaci�n.
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
        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n GET para verificar permisos de acceso.
        /// ��</summary>_PLACEHOLDER��
        /// <returns>Permite el acceso solo a administradores; de lo contrario, devuelve prohibici�n.</returns>

        public IActionResult OnGet()
        {
            // Verificar que solo los administradores pueden acceder a esta p�gina
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return Page();
        }

        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n POST para procesar la creaci�n de un nuevo juego.
        /// ��</summary>_PLACEHOLDER��
        /// <returns>Redirige a la p�gina principal tras creaci�n exitosa o recarga la p�gina con errores.</returns>
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
