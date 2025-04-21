using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace M09Practica.Pages
{
    [Authorize]
    public class ChatModel : PageModel
    {
        public void OnGet()
        {
            // No necesitas lógica especial aquí
        }
    }
}
