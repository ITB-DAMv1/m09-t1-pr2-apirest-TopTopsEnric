namespace M09Practica.Model.Request
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO para la solicitud de registro de usuario, que contiene email, contraseña, nombre, apellido y nombre de usuario.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class RegisterUserRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string Username { get; set; }
    }
}
