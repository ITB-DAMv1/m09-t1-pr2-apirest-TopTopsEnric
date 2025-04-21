namespace M09Practica.Model.ViewModel
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de vista que representa un juego para la interfaz de usuario, incluyendo identificador, nombre, descripción, imagen, votos y propiedades para mostrar descripción truncada y texto de votos.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Votes { get; set; }

        // Propiedades para la UI
        public string TruncatedDescription => Description?.Length > 100
            ? Description.Substring(0, 97) + "..."
            : Description;

        public string VotesDisplay => $"{Votes} {(Votes == 1 ? "voto" : "votos")}";
    }
}
