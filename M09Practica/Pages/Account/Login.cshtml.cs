using M09Practica.Model.Forms;
using M09Practica.Model.Request;
using M09Practica.Model.Response;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace M09Practica.Pages.Account
{
/// ‡‡<summary>_PLACEHOLDER‡‡
/// Modelo de página para el inicio de sesión de usuarios.
/// ‡‡</summary>_PLACEHOLDER‡‡
/// <remarks>
/// Gestiona la autenticación mediante llamada a la API externa.
/// Maneja la creación del token, almacenamiento en sesión y autenticación con cookies.
/// Redirige a la página principal si el usuario ya está autenticado.
/// </remarks>
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Propiedad enlazada que contiene los datos del formulario de login.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        [BindProperty]
        public LoginFormModel LoginData { get; set; }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Mensaje de error para mostrar en la vista en caso de fallo de autenticación.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        public string ErrorMessage { get; set; }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Constructor que recibe la fábrica de clientes HTTP para llamadas a la API.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <param name="clientFactory">Fábrica para crear clientes HTTP.</param>
        public LoginModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición GET.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Redirige a la página principal si el usuario ya está autenticado.
        /// </remarks>
        public void OnGet()
        {
            // Si ya está autenticado, redirigir a la página principal
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Index");
            }
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que se ejecuta en la petición POST para procesar el inicio de sesión.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <returns>Redirige a la página principal si el login es exitoso; de lo contrario, recarga la página con errores.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("GameAPI");

            var loginDto = new LoginRequestDto
            {
                Email = LoginData.Email,
                Password = LoginData.Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginDto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/Auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponseDto>(responseContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Guardar el token en la sesión
                HttpContext.Session.SetString("AuthToken", tokenResponse.Token);

                // Crear claims para la autenticación
                var claims = new List<Claim>
            {
                  new Claim(ClaimTypes.Name, tokenResponse.Token),  // O el campo correcto que contenga el nombre
                   new Claim(ClaimTypes.Role, tokenResponse.Role)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = false,
                    });

                return RedirectToPage("/Index");
            }

            ModelState.AddModelError(string.Empty, "Nombre de usuario o contraseña incorrectos");
            return Page();
        }
    }
}
