using System.ComponentModel.DataAnnotations;

namespace ApiM09practica.DTO
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO para crear un juego, que incluye el nombre, la descripción y la URL de la imagen de perfil.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        [Url] 
        public string ProfileImage { get; set; }
    }
}
