namespace M09Practica.Model.ViewModel
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de vista que representa los detalles de un juego, incluyendo su identificador, nombre, descripción, URL de imagen, votos y si el usuario puede votar.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameDetailViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Votes { get; set; }

        // Propiedades adicionales si son necesarias
        public bool CanVote { get; set; } // Para controlar si el usuario puede votar
    }
}
