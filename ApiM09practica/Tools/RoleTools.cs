using Microsoft.AspNetCore.Identity;

namespace ApiM09practica.Tools
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Clase estática que contiene herramientas para la gestión de roles en la aplicación.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public static class RoleTools
    {

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Función que crea los roles iniciales ("Admin" y "User") si no existen en el sistema.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Utiliza el RoleManager para verificar la existencia de cada rol y los crea en caso de que no existan.
        /// </remarks>
        public static async Task CrearRolsInicials(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] rols = { "Admin", "User" };

            foreach (var rol in rols)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }
        }
    }
}
