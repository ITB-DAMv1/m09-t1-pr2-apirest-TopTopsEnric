using System.ComponentModel.DataAnnotations;

namespace M09Practica.Model.Forms
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo para el formulario de creación o edición de un juego, con validaciones para nombre, descripción y URL de imagen.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class GameFormModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
        [Display(Name = "Nombre del juego")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(250, ErrorMessage = "La descripción no puede tener más de 250 caracteres")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La URL de la imagen es obligatoria")]
        [Url(ErrorMessage = "Debe ser una URL válida")]
        [Display(Name = "URL de la imagen")]
        public string ProfileImage { get; set; }
    }
}
