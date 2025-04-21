using M09Practica.Model.Response;
using M09Practica.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace M09Practica.Pages.Games
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de página para mostrar los detalles de un juego específico.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Obtiene los datos del juego desde la API y los muestra en la vista.
    /// Permite a usuarios autenticados votar por el juego.
    /// Permite a administradores eliminar el juego.
    /// Maneja errores de carga, votación y eliminación.
    /// </remarks>
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public GameDetailViewModel Game { get; set; }
        public string ErrorMessage { get; set; }

        public DetailsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición GET para cargar los detalles del juego.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <param name="id">Identificador del juego.</param>
        /// <returns>Muestra la página con los detalles o error si no se encuentra el juego.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("GameAPI");
            var response = await client.GetAsync($"api/Game/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gameDto = JsonSerializer.Deserialize<GameDto>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Game = new GameDetailViewModel
                {
                    Id = gameDto.Game_ID,
                    Name = gameDto.Name,
                    Description = gameDto.Description,
                    ImageUrl = gameDto.ProfileImage,
                    Votes = gameDto.Votos,
                    CanVote = User.Identity.IsAuthenticated
                };

                return Page();
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            ErrorMessage = $"Error al cargar el juego: {response.StatusCode}";
            return Page();
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición POST para votar por un juego.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <param name="id">Identificador del juego.</param>
        /// <returns>Redirige a la misma página con el resultado de la votación o muestra error.</returns>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login");
            }

            var client = _clientFactory.CreateClient("GameAPI");

            // Obtener el token del usuario
            var token = HttpContext.Session.GetString("AuthToken");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsync($"api/Game/{id}/vote", null);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Games/Details", new { id });
            }

            // Manejar error
            var content = await response.Content.ReadAsStringAsync();
            ErrorMessage = $"Error al votar: {content}";

            // Recargar los datos del juego
            await OnGetAsync(id);
            return Page();
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición POST para eliminar un juego.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <param name="id">Identificador del juego.</param>
        /// <returns>Redirige a la página principal tras eliminación o muestra error.</returns>
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            // Verificar permisos
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var client = _clientFactory.CreateClient("GameAPI");

            // Obtener el token del usuario actual
            var token = HttpContext.Session.GetString("AuthToken");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"api/Game/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Index");
            }

            // Manejar error
            var responseContent = await response.Content.ReadAsStringAsync();
            TempData["ErrorMessage"] = $"Error al eliminar: {responseContent}";
            return RedirectToPage("/Games/Details", new { id });
        }

    }
}
