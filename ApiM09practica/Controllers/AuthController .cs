using ApiM09practica.models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiM09practica.DTO;
using Microsoft.AspNetCore.Authorization;

namespace ApiM09practica.Controllers
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Clase encargada de crear los endpoints para la  autenticacion y registro de usuarios.
    /// ‡‡</summary>_PLACEHOLDER‡‡
    /// <remarks>
    /// Estan incluidos los basicos de registrar tanto para user como admin y el login y la funcion de creacion de token.
    ///Tambien se incluyo uno para ver si recibiamos el token bien porque en el desarrollo me daba problemas
    /// </remarks>

    [ApiController]
    [Route("api/[Controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        public AuthController(UserManager<ApplicationUser> userManager, ILogger<AuthController> logger, IConfiguration configuration)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Funcion encargada de registrar un usuario. 
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        ///  Recibe el modelo RegisterDTO y lo valida.
        ///Crea el usuario y le asigna el rol de User.
        ///Hashea la contraseña y el rol funciona ingresa en la base de datos, si falla elimina todo para evitar problemas.
        /// </remarks>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO model)
        {
            // Verifica si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { error = "El correo electrónico ya está registrado" });
            }

            // Crea el nuevo usuario
            var usuari = new ApplicationUser
            {
                UserName = model.Username,    // UserName debe ser único, usar Email es una buena práctica
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname
            };

            // CreateAsync hashea la contraseña automáticamente
            var resultat = await _userManager.CreateAsync(usuari, model.Password);

            if (resultat.Succeeded)
            {
                // Asigna el rol de usuario
                var resultatRol = await _userManager.AddToRoleAsync(usuari, "User");

                if (resultatRol.Succeeded)
                {
                    return Ok(new { message = "Usuari registrat correctament" });
                }
                else
                {
                    // Si falla la asignación de rol, elimina el usuario creado
                    await _userManager.DeleteAsync(usuari);
                    return BadRequest(resultatRol.Errors);
                }
            }

            return BadRequest(resultat.Errors);
        }


        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Funcion encargada de registrar un usuario_admin. 
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        ///  Recibe el modelo RegisterDTO y lo valida.
        ///Crea el usuario y le asigna el rol de admin.
        ///Hashea la contraseña y el rol funciona ingresa en la base de datos, si falla elimina todo para evitar problemas.
        /// </remarks>
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDTO model)
        {
            // Verifica si el usuario ya existe
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { error = "El Administrador  ya está registrado" });
            }

            // Crea el nuevo usuario administrador
            var usuari = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                Name = model.Name,
                Surname = model.Surname
            };

            // CreateAsync hashea la contraseña automáticamente
            var resultat = await _userManager.CreateAsync(usuari, model.Password);

            if (resultat.Succeeded)
            {
                // Asigna el rol de administrador
                var resultatRol = await _userManager.AddToRoleAsync(usuari, "Admin");

                if (resultatRol.Succeeded)
                {
                    return Ok(new { message = "Administrador registrat correctament" });
                }
                else
                {
                    // Si falla la asignación de rol, elimina el usuario creado
                    await _userManager.DeleteAsync(usuari);
                    return BadRequest(resultatRol.Errors);
                }
            }

            return BadRequest(resultat.Errors);
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Funcion encargada de realizar el Login. 
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Recibe el modelo LoginDTO y mira que exista el usuario.
        ///Si existe comprueba la contraseña.
        ///luego seteamos los claims que son los datos que se guardan en el token.
        /// Finalmente creamos el token y lo retornamos.
        /// </remarks>

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            //Certifiquem que el mail existeix
            var usuari = await _userManager.FindByEmailAsync(model.Email);
            if (usuari == null || !await _userManager.CheckPasswordAsync(usuari, model.Password))
                return Unauthorized("Mail o contrasenya erronis");

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuari.UserName),
                new Claim(ClaimTypes.NameIdentifier, usuari.Id.ToString())
            };

            //Adquirim els rols de l'usuari. Pot tenir mes d'un. En aquest cas 1.
            var roles = await _userManager.GetRolesAsync(usuari);

            if (roles != null && roles.Count > 0)
            {
                foreach (var rol in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, rol));
                }
            }

            return Ok(CreateToken(claims.ToArray()));
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Funcion encargada de crear el Token. 
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Recoje todas los datos que el token necesita y que queremos que tenga.
        ///Genera la llave con el codigo de encriptacion que queremos y con la llave que queramos.
        ///Creamos el token y lo metemos en un string y lo devolvemos.
        /// </remarks>
        private object CreateToken(Claim[] claims)
        {
            // Carreguem les dades des del appsettings.json
            var jwtConfig = _configuration.GetSection("JwtSettings");
            var secretKey = jwtConfig["Key"];
            var issuer = jwtConfig["Issuer"];
            var audience = jwtConfig["Audience"];
            var expirationMinutes = int.Parse(jwtConfig["ExpirationMinutes"]);

            Console.WriteLine($"Token generation - Key: {secretKey?.Length} chars");
            Console.WriteLine($"Token generation - Issuer: {issuer}");
            Console.WriteLine($"Token generation - Audience: {audience}");

            // Creem la clau i les credencials de signatura
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Construcció del token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"Token generado: {tokenString}");
            return new
            {
                Token = tokenString,
                Expiration = token.ValidTo,
                Role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value
            };
            
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Funcion encargada de hacer pruebas si el token estaba funcionando. 
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Confirmaos que la autorizacion funciona y que extrae el token correctamente.
        /// </remarks>
        [HttpGet("debug-token")]
        [AllowAnonymous]
        public IActionResult DebugToken()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader))
            {
                return BadRequest("No Authorization header found");
            }

            Console.WriteLine($"Authorization header: {authHeader}");

            // Extraer el token correctamente
            string token;
            if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                token = authHeader.Substring("Bearer ".Length).Trim();
            }
            else
            {
                token = authHeader.Trim();
            }

            Console.WriteLine($"Token extraído: {token}");

            return Ok(new { TokenLength = token.Length, TokenStart = token.Substring(0, Math.Min(10, token.Length)) });
        }

    }
}
