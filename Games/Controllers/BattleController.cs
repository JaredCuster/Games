using Games.Models;
using Games.Services.ControllerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BattleController : ControllerBase
    {
        private IBattleControllerService _battleService;

        public BattleController(IBattleControllerService battleService)
        {
            _battleService = battleService;
        }

        /// <summary>
        /// Get the Battles
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the battles</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Must be an Admin</response>
        [HttpGet]
        [Route("/api/Battles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Battle>))]
        public async Task<IActionResult> GetAll()
        {
            var battles = await _battleService.GetBattlesAsync();

            return Ok(battles);
        }

        /// <summary>
        /// Get a specific battle
        /// </summary>
        /// <param name="id">The id of the battle</param>
        /// <returns></returns>
        /// <response code="200">Returns the battle</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Must be an Admin</response>
        /// <response code="404">Battle not found</response>
        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Battle))]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var battle = await _battleService.GetBattleAsync(id);
                return Ok(battle);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Start a battle
        /// </summary>
        /// <param name="character1Id">The id of the character starting the battle (you must own this character)</param>
        /// <param name="character2Id">The id of the other character involved in the battle</param>
        /// <returns></returns>
        /// <response code="200">Returns the battle</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Invalid battle</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Character))]
        public async Task<IActionResult> Add([FromQuery] int character1Id, [FromQuery] int character2Id)
        {
            try
            {
                var battle = await _battleService.AddBattleAsync(character1Id, character2Id);

                return Ok(battle);
            }
            catch (BattleException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Make a battle move
        /// </summary>
        /// <param name="id">The id of the battle</param>
        /// <param name="characterId">The id of the character doing the move</param>
        /// <param name="move">The battle move</param>
        /// <returns></returns>
        /// <remarks>
        /// Valid Moves: Accept, Attack, Pursue, Retreat, Surrender, Quit
        /// </remarks>
        /// <response code="200">Returns the battle</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Battle, opponent or move not found</response>
        /// <response code="400">Invalid battle move</response>
        [HttpPost]
        [Route("{id:int}/Move")]
        public async Task<IActionResult> AddMove([FromRoute] int id, int characterId, string move)
        {
            try
            {
                var results = await _battleService.AddBattleMoveAsync(id, characterId, move);

                return Ok(results);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (BattleException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
