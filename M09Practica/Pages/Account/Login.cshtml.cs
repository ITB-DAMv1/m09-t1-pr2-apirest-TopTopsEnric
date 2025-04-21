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
/// ��<summary>_PLACEHOLDER��
/// Modelo de p�gina para el inicio de sesi�n de usuarios.
/// ��</summary>_PLACEHOLDER��
/// <remarks>
/// Gestiona la autenticaci�n mediante llamada a la API externa.
/// Maneja la creaci�n del token, almacenamiento en sesi�n y autenticaci�n con cookies.
/// Redirige a la p�gina principal si el usuario ya est� autenticado.
/// </remarks>
    public class LoginModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        /// ��<summary>_PLACEHOLDER��
        /// Propiedad enlazada que contiene los datos del formulario de login.
        /// ��</summary>_PLACEHOLDER��
        [BindProperty]
        public LoginFormModel LoginData { get; set; }

        /// ��<summary>_PLACEHOLDER��
        /// Mensaje de error para mostrar en la vista en caso de fallo de autenticaci�n.
        /// ��</summary>_PLACEHOLDER��
        public string ErrorMessage { get; set; }
        /// ��<summary>_PLACEHOLDER��
        /// Constructor que recibe la f�brica de clientes HTTP para llamadas a la API.
        /// ��</summary>_PLACEHOLDER��
        /// <param name="clientFactory">F�brica para crear clientes HTTP.</param>
        public LoginModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n GET.
        /// ��</summary>_PLACEHOLDER��
        /// <remarks>
        /// Redirige a la p�gina principal si el usuario ya est� autenticado.
        /// </remarks>
        public void OnGet()
        {
            // Si ya est� autenticado, redirigir a la p�gina principal
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/Index");
            }
        }

        /// ��<summary>_PLACEHOLDER��
        /// M�todo que se ejecuta en la petici�n POST para procesar el inicio de sesi�n.
        /// ��</summary>_PLACEHOLDER��
        /// <returns>Redirige a la p�gina principal si el login es exitoso; de lo contrario, recarga la p�gina con errores.</returns>
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

                // Guardar el token en la sesi�n
                HttpContext.Session.SetString("AuthToken", tokenResponse.Token);

                // Crear claims para la autenticaci�n
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

            ModelState.AddModelError(string.Empty, "Nombre de usuario o contrase�a incorrectos");
            return Page();
        }
    }
}
