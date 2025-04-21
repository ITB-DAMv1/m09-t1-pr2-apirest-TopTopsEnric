namespace M09Practica.Model.Response
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// DTO que representa la respuesta de un token de autenticación, incluyendo el token, su fecha de expiración y el rol del usuario.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class TokenResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string Role { get; set; }
    }
}
