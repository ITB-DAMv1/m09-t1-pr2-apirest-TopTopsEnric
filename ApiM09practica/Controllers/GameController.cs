using ApiM09practica.Contexts;
using ApiM09practica.DTO;
using ApiM09practica.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;

namespace ApiM09practica.Controllers
{
    /// ‡‡<summary>_PLACEHOLDER‡‡
    /// Controlador que maneja las operaciones relacionadas con los juegos.
    /// ‡‡</summary>_PLACEHOLDER‡‡

    [ApiController]
    [Route("api/[Controller]")]
    public class GameController: ControllerBase
    {
        private readonly dbContexts _context;

        public GameController(dbContexts context)
        {
            _context = context;
        }


        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Obtiene todos los juegos.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Consulta la base de datos para traer todos los juegos y los transforma en objetos que se pueden enviar al cliente.
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameReadDto>>> GetAllGames()
        {
            var games = await _context.games
                //.Include(f => f.Name)
                .Select(f => new GameReadDto
                {
                    Game_ID = f.Game_ID,
                    Name = f.Name,
                    Description = f.Description,
                    ProfileImage = f.ProfileImage,
                    Votos = f.votos
                })
                .ToListAsync();
            return Ok(games);
        }
        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Obtiene un juego por su ID.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Busca el juego en la base de datos usando el ID.
        /// Si no lo encuentra, devuelve un error.
        /// Si lo encuentra, lo convierte al formato que se envía al cliente.
        /// </remarks>

        [HttpGet("{id}")]
        public async Task<ActionResult<GameReadDto>> GetGame(int id)
        {
            var game = await _context.games.FindAsync(id);

            if (game == null)
            {
                return NotFound();
            }

            // Deberías mapear a GameReadDto para consistencia
            var gameDto = new GameReadDto
            {
                Game_ID = game.Game_ID,
                Name = game.Name,
                Description = game.Description,
                ProfileImage = game.ProfileImage,
                Votos = game.votos
            };

            return Ok(gameDto);
        }


        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Crea un nuevo juego (solo para administradores).
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Recibe los datos del juego, crea un nuevo registro con esos datos y el ID del usuario que lo crea.
        /// Intenta guardar el juego en la base de datos y maneja posibles errores.
        /// Devuelve el juego creado con su nuevo ID.
        /// </remarks>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<GameReadDto>> PostGame(GameCreateDto gameDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var joc = new Games
            {
                Name = gameDTO.Name,
                Description = gameDTO.Description,
                ProfileImage = gameDTO.ProfileImage,
                votos = 0,
                UserId = userId
            };

            try
            {
                await _context.games.AddAsync(joc);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Registrar la excepción interna
                Console.WriteLine($"Error interno: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.InnerException?.StackTrace}");

                return StatusCode(500, new { message = $"Error al crear el juego: {ex.InnerException?.Message}" });
            }

            var gameReadDto = new GameReadDto
            {
                Game_ID = joc.Game_ID,
                Name = joc.Name,
                Description = joc.Description,
                ProfileImage = joc.ProfileImage,
                Votos = joc.votos
            };

            return CreatedAtAction(nameof(GetGame), new { id = joc.Game_ID }, gameReadDto);
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Elimina un juego por su ID (solo para administradores).
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Busca el juego por ID y, si existe, lo elimina de la base de datos.
        /// Maneja errores que puedan ocurrir al eliminar.
        /// </remarks>

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            try
            {
                _context.games.Remove(game);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex);
            }
            return NoContent();
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Actualiza un juego existente (solo para administradores).
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Busca el juego por ID y actualiza solo los campos que vienen con datos nuevos.
        /// Guarda los cambios en la base de datos y maneja posibles errores de concurrencia.
        /// </remarks>

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameUpdateDto gameDto)
        {
            var game = await _context.games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            
            if (!string.IsNullOrEmpty(gameDto.Name))
                game.Name = gameDto.Name;

            if (!string.IsNullOrEmpty(gameDto.Description))
                game.Description = gameDto.Description;

            if (!string.IsNullOrEmpty(gameDto.ProfileImage))
                game.ProfileImage = gameDto.ProfileImage;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GameExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }


        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Añade un voto a un juego (para administradores y usuarios).
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Busca el juego por ID, incrementa su contador de votos y guarda el cambio.
        /// Maneja posibles errores al guardar.
        /// </remarks>
        [Authorize(Roles = "Admin,User")] // Permitir Admin y User
        [HttpPost("{id}/vote")]
        public async Task<IActionResult> VoteGame(int id)
        {
            var game = await _context.games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }

            // si da tiempo tal vez deberia implemnetar que la tabla votos fuera tabla intermedia entre usuario y juegos

            game.votos++;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex);
            }

            return Ok(new { game.Game_ID, game.Name, Votos = game.votos });
        }

        /// ‡‡<summary>_PLACEHOLDER‡‡
        /// Verifica si un juego existe por su ID.
        /// ‡‡</summary>_PLACEHOLDER‡‡
        /// <remarks>
        /// Consulta la base de datos para saber si hay un juego con ese ID.
        /// </remarks>

        private bool GameExists(int id)
        {
            return _context.games.Any(e => e.Game_ID == id);
        }
    }
}
