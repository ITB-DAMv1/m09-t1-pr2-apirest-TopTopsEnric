namespace M09Practica.Model.Response
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO que representa un juego con su identificador, nombre, descripción, imagen de perfil y número de votos.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameDto
    {
        public int Game_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfileImage { get; set; }
        public int Votos { get; set; }
    }
}
