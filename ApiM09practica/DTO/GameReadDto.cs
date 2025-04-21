using System.ComponentModel.DataAnnotations;

namespace ApiM09practica.DTO
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO para leer los datos de un juego, que incluye el ID, nombre, descripción, URL de la imagen de perfil y la cantidad de votos.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameReadDto
    {
        public int Game_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Url]
        public string ProfileImage { get; set; }
        public int Votos { get; set; }
        
    }
}
