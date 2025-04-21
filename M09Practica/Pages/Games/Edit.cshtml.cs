using M09Practica.Model.Response;
using M09Practica.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;
using M09Practica.Model.Request;

namespace M09Practica.Pages.Games
{
    /// ��<summary>_PLACEHOLDER��
    /// Modelo de p�gina para editar los datos de un juego existente, accesible solo para administradores.
    /// ��</summary>_PLACEHOLDER��
    /// <remarks>
    /// Carga los datos actuales del juego desde la API para su edici�n.
    /// Env�a los datos actualizados a la API con autenticaci�n mediante token.
    /// Maneja errores de carga y actualizaci�n.
    /// </remarks>
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public GameEditViewModel GameData { get; set; }

        public string ErrorMessage { get; set; }

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n GET para cargar los datos del juego a editar.
        /// ��</summary>_PLACEHOLDER��
        /// <param name="id">Identificador del juego.</param>
        /// <returns>Muestra la p�gina con los datos del juego o error si no se encuentra.</returns>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Verificar permisos
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var client = _clientFactory.CreateClient("GameAPI");
            var response = await client.GetAsync($"api/Game/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gameDto = JsonSerializer.Deserialize<GameDto>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                GameData = new GameEditViewModel
                {
                    Id = gameDto.Game_ID,
                    Name = gameDto.Name,
                    Description = gameDto.Description,
                    ProfileImage = gameDto.ProfileImage
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

        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n POST para enviar los datos actualizados del juego.
        /// ��</summary>_PLACEHOLDER��
        /// <returns>Redirige a la p�gina de detalles tras actualizaci�n exitosa o recarga la p�gina con errores.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            // Verificar permisos
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

            // Crear DTO que coincida con GameUpdateDto del backend
            var updateDto = new GameUpdateDto
            {
                Name = GameData.Name,
                Description = GameData.Description,
                ProfileImage = GameData.ProfileImage
            };

            var content = new StringContent(
                JsonSerializer.Serialize(updateDto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync($"api/Game/{GameData.Id}", content);

            // El backend retorna NoContent (204) si es exitoso
            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Games/Details", new { id = GameData.Id });
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error al actualizar el juego: {responseContent}");
            return Page();
        }
    }
}
