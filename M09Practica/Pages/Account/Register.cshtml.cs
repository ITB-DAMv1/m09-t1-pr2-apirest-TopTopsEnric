using M09Practica.Model.Forms;
using M09Practica.Model.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace M09Practica.Pages.Account
{
    /// ��<summary>_PLACEHOLDER��
    /// Modelo de p�gina para el registro de nuevos usuarios.
    /// ��</summary>_PLACEHOLDER��
    /// <remarks>
    /// Gestiona la creaci�n de usuarios mediante llamada a la API externa.
    /// Redirige a la p�gina de login tras un registro exitoso.
    /// Si el usuario ya est� autenticado, redirige a la p�gina principal.
    /// </remarks>
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        /// ��<summary>_PLACEHOLDER��
        /// Propiedad enlazada que contiene los datos del formulario de registro.
        /// ��</summary>_PLACEHOLDER��
        [BindProperty]
        public RegisterUserFormModel RegisterData { get; set; }
        /// ��<summary>_PLACEHOLDER��
        /// Mensaje de error para mostrar en la vista en caso de fallo en el registro.
        /// ��</summary>_PLACEHOLDER��
        public string ErrorMessage { get; set; }
        /// ��<summary>_PLACEHOLDER��
        /// Constructor que recibe la f�brica de clientes HTTP para llamadas a la API.
        /// ��</summary>_PLACEHOLDER��
        /// <param name="clientFactory">F�brica para crear clientes HTTP.</param>
        public RegisterModel(IHttpClientFactory clientFactory)
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
        /// M�todo que se ejecuta en la petici�n POST para procesar el registro de usuario.
        /// ��</summary>_PLACEHOLDER��
        /// <returns>Redirige a la p�gina de login si el registro es exitoso; de lo contrario, recarga la p�gina con errores.</returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("GameAPI");

            var registerDto = new RegisterUserRequestDto
            {
                Username = RegisterData.Username,
                Name = RegisterData.Name,         
                Surname = RegisterData.Surname,
                Email = RegisterData.Email,
                Password = RegisterData.Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(registerDto),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/Auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Account/Login");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"Error al registrarse: {responseContent}");
            return Page();
        }
    }
}
