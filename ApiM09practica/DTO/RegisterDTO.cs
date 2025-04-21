using System.ComponentModel.DataAnnotations;

namespace ApiM09practica.DTO
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO para el registro de usuarios, que incluye correo electrónico, contraseña, nombre, apellido y nombre de usuario.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class RegisterDTO
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Username { get; set; }
    }
}
