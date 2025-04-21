using Microsoft.AspNetCore.Authentication.Cookies;

namespace M09Practica
{
    /// ��<summary>_PLACEHOLDER��
    /// Clase principal que configura y ejecuta la aplicaci�n web.
    /// ��</summary>_PLACEHOLDER��
    /// <remarks>
    /// Configura servicios como Razor Pages, HttpClient para la API de juegos, autenticaci�n con cookies, autorizaci�n, y sesiones.
    /// Define el pipeline de solicitudes HTTP con manejo de errores, HTTPS, archivos est�ticos, sesiones, autenticaci�n y autorizaci�n.
    /// Finalmente, inicia la aplicaci�n.
    /// </remarks>
    public class Program
    {
        /// ��<summary>_PLACEHOLDER��
        /// M�todo principal asincr�nico que construye y ejecuta la aplicaci�n.
        /// ��</summary>_PLACEHOLDER��
        /// <param name="args">Argumentos de l�nea de comandos.</param>
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Esperamos a que la API est� lista
            await Task.Delay(3000);

            // Add services to the container.
            builder.Services.AddRazorPages();

            // Acc�s a la configuraci� del fitxer appsettings.json
            string apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API base URL not found");
            int timeout = int.Parse(builder.Configuration["ApiSettings:TimeoutSeconds"] ?? "30");

            // Configuramos el HttpClient para la API de Juegos 
            builder.Services.AddHttpClient("GameAPI", client =>
            {
                client.BaseAddress = new Uri(apiBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(timeout);
            });

            // Configuramos autenticaci�n con cookies
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";  // Ruta correcta a la p�gina de login
                    options.AccessDeniedPath = "/Account/AccessDenied";
                   
                });

            // Agregamos autorizaci�n
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdminRole", policy =>
                    policy.RequireRole("Admin"));
            });

            // Configuramos sesiones
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Activamos sesiones antes del enrutamiento
            app.UseSession();

            app.UseRouting();

            // Agregamos autenticaci�n y autorizaci�n al pipeline
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
