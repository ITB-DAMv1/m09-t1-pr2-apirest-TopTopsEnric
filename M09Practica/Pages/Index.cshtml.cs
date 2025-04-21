using M09Practica.Model.Response;
using M09Practica.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace M09Practica.Pages
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de página para mostrar la lista de juegos disponibles.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Obtiene la lista de juegos desde la API.
    /// Mapea los datos recibidos a la vista y ordena los juegos por número de votos en orden descendente.
    /// Guarda el token JWT de la sesión para posibles usos en la vista.
    /// Maneja errores en la carga de datos.
    /// </remarks>
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public List<GameViewModel> Games { get; set; } = new List<GameViewModel>();
        public string ErrorMessage { get; set; }
        public string JwtToken { get; set; }
        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición GET para cargar la lista de juegos.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("GameAPI");
            var response = await client.GetAsync("api/Game");
            JwtToken = HttpContext.Session.GetString("AuthToken");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var gameDtos = JsonSerializer.Deserialize<List<GameDto>>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Mapear y ordenar por votos (descendente)
                Games = gameDtos
                    .Select(g => new GameViewModel
                    {
                        Id = g.Game_ID,
                        Name = g.Name,
                        Description = g.Description,
                        ImageUrl = g.ProfileImage,
                        Votes = g.Votos
                    })
                    .OrderByDescending(g => g.Votes)
                    .ToList();
            }
            else
            {
                ErrorMessage = $"Error al cargar juegos: {response.StatusCode}";
            }
        }
    }
}
