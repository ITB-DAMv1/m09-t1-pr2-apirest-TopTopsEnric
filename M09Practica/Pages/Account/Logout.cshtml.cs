using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace M09Practica.Pages.Account
{
    /// ��<summary>_PLACEHOLDER��
    /// Modelo de p�gina para gestionar el cierre de sesi�n del usuario.
    /// ��</summary>_PLACEHOLDER��
    /// <remarks>
    /// Limpia la sesi�n, cierra la autenticaci�n basada en cookies,
    /// elimina las cookies relacionadas y redirige a la p�gina principal.
    /// </remarks>
    public class LogoutModel : PageModel
    {
        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n GET para cerrar la sesi�n del usuario.
        /// ��</summary>_PLACEHOLDER��
        /// <returns>Redirige a la p�gina principal tras cerrar sesi�n.</returns>
        public async Task<IActionResult> OnGetAsync()
        {
            // Limpiar la sesi�n
            HttpContext.Session.Clear();

            // Cerrar la autenticaci�n (cookie)
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Eliminar cookies de sesi�n y autenticaci�n
            if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Session"))
            {
                Response.Cookies.Delete(".AspNetCore.Session");
            }
            if (HttpContext.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
            {
                Response.Cookies.Delete(".AspNetCore.Cookies");
            }

            // Redirigir a la p�gina principal
            return RedirectToPage("/Index");
        }
    }
}
