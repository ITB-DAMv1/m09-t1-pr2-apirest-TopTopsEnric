using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace M09Practica.Pages
{
    /// ��<summary>_PLACEHOLDER��
    /// Modelo de p�gina para manejar y mostrar errores en la aplicaci�n.
    /// ��</summary>_PLACEHOLDER��
    /// <remarks>
    /// Deshabilita el almacenamiento en cach� de la respuesta y omite la verificaci�n antifalsificaci�n.
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

        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n GET para asignar el identificador de la solicitud actual.
        /// ��</summary>_PLACEHOLDER��
        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }

}
