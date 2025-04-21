using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ApiM09practica.models
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Clase que representa un juego, con propiedades para ID, nombre, descripción, imagen de perfil, votos y la relación con el usuario propietario.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class Games
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Game_ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        [MaxLength(250)]
        public string Description { get; set; } = string.Empty;

        public string ProfileImage { get; set; }

        public int votos { get; set; }

        [Required]
        public string UserId { get; set; }

       

        [ForeignKey("UserId")]
        public ApplicationUser user { get; set; } = null;
    }
}
