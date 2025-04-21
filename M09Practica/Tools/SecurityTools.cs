using System.Security.Cryptography;
using System.Text;

namespace M09Practica.Tools
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Clase estática que proporciona herramientas de seguridad.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    public class SecurityTools
    {

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Método que encripta una contraseña utilizando el algoritmo SHA-256.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <param name="password">Contraseña en texto plano a encriptar.</param>
        /// <returns>Cadena hexadecimal que representa el hash SHA-256 de la contraseña.</returns>
        public static string EncriptPassword(string password)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] hashedBytes = hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convertir bytes a cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    builder.Append(hashedBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
