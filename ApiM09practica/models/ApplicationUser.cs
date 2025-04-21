using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ApiM09practica.models
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Clase que extiende IdentityUser para representar un usuario de la aplicación, incluyendo nombre, apellido y una lista de videojuegos asociados.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class ApplicationUser : IdentityUser
    {
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public List<Games>? videoGames { get; set; }
    }
}
