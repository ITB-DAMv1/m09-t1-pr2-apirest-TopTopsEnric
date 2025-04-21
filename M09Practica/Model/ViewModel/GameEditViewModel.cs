using System.ComponentModel.DataAnnotations;

namespace M09Practica.Model.ViewModel
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo de vista para la edición de un juego, que incluye el identificador, nombre, descripción y URL de la imagen con validaciones de longitud y formato.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameEditViewModel
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre del juego")]
        public string Name { get; set; }

        [StringLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Debe ser una URL válida")]
        [Display(Name = "URL de la imagen")]
        public string ProfileImage { get; set; }
    }
}
