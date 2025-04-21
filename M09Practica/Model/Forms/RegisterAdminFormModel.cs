using System.ComponentModel.DataAnnotations;

namespace M09Practica.Model.Forms
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo para el formulario de registro de administrador, con campos para nombre de usuario, nombre, apellido, correo electrónico, contraseña y confirmación de contraseña, incluyendo validaciones.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class RegisterAdminFormModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [Display(Name = "Nombre de usuario")]
        public string Username { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "apellido de usuario")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "El nombre  es obligatorio")]
        [Display(Name = "Nombre ")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres de longitud.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
        public string ConfirmPassword { get; set; }

    }
}
