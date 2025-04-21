using System.ComponentModel.DataAnnotations;

namespace ApiM09practica.DTO
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO para actualizar un juego, con campos opcionales para nombre, descripción y URL de la imagen de perfil.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameUpdateDto
    {
        
        [MaxLength(50)]
        public string? Name { get; set; }

       
        [MaxLength(250)]
        public string? Description { get; set; }

        
        public string? ProfileImage { get; set; }
    }
}
