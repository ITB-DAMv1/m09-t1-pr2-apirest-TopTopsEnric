using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace M09Practica.Pages
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de página para manejar y mostrar errores en la aplicación.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Deshabilita el almacenamiento en caché de la respuesta y omite la verificación antifalsificación.
    /// Obtiene el identificador de la solicitud para mostrarlo en la vista, facilitando la trazabilidad del error.
    /// </remarks>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición GET para asignar el identificador de la solicitud actual.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

}
