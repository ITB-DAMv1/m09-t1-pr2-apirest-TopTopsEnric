using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ApiM09practica.Hubs
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Hub de SignalR encargado de gestionar la comunicación en tiempo real del chat.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    [Authorize]
    public class Chathub:Hub
    {

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Función que envía un mensaje recibido a todos los clientes conectados.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Obtiene el nombre del usuario que envía el mensaje o asigna "Anònim" si no está identificado.
        /// Luego transmite el mensaje a todos los clientes suscritos al evento "RepMissatge".
        /// </remarks>
        public async Task EnviaMissatge(string missatge)
        {
            var usuari = Context.User.Identity.Name ?? "Anònim";
            await Clients.All.SendAsync("RepMissatge", usuari, missatge);
        }

    }
}
