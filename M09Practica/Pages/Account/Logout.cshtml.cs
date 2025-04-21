using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace M09Practica.Pages.Account
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de página para gestionar el cierre de sesión del usuario.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Limpia la sesión, cierra la autenticación basada en cookies,
    /// elimina las cookies relacionadas y redirige a la página principal.
    /// </remarks>
    public class LogoutModel : PageModel
    {
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición GET para cerrar la sesión del usuario.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <returns>Redirige a la página principal tras cerrar sesión.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();

            // Cerrar la autenticación (cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Eliminar cookies de sesión y autenticación
            if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Session"))
            {
                Response.Cookies.Delete(".AspNetCore.Session");
            }
            if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
            {
                Response.Cookies.Delete(".AspNetCore.Cookies");
            }

            // Redirigir a la página principal
            return RedirectToPage("/Index");
        }
    }
}
