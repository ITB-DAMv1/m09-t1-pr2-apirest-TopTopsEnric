using System.IdentityModel.Tokens.Jwt;

namespace M09Practica.Tools
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Clase auxiliar para operaciones relacionadas con tokens JWT.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class TokenHelper
    {
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Verifica si un token de sesión es válido y no ha expirado.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <param name="token">Token JWT en formato string.</param>
        /// <returns>True si el token no está vacío y no ha expirado; false en caso contrario.</returns>
        public static bool IsTokenSession(string token)
        {
            return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Valida si un token JWT ha expirado.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Requiere la librería System.IdentityModel.Tokens.Jwt para su funcionamiento.
        /// </remarks>
        /// <param name="token">Token JWT en formato string.</param>
        /// <returns>True si el token ha expirado; false si aún es válido.</returns>
        public static bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var expiration = jwt.ValidTo;
            return expiration < DateTime.UtcNow;
        }
    }
}
