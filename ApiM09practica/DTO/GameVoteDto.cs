using System.ComponentModel.DataAnnotations;

namespace ApiM09practica.DTO
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO para votar un juego, que incluye el ID del juego (obligatorio).
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameVoteDto
    {
        [Required]
        public int Game_ID { get; set; }
    }
}
