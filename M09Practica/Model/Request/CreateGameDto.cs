namespace M09Practica.Model.Request
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO para la creación de un juego, que contiene el nombre, descripción y URL de la imagen del juego.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class CreateGameDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ProfileImage { get; set; }
    }
}
