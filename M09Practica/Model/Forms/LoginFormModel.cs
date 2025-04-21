using System.ComponentModel.DataAnnotations;

namespace M09Practica.Model.Forms
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Modelo para el formulario de login, que contiene el email y la contraseña del usuario con sus validaciones.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class LoginFormModel
    {
        [Required(ErrorMessage = "El email de usuario es obligatorio")]
        [Display(Name = "Email de usuario")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        
       
    }
}
