using Games.Models;
using Games.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BattleController : ControllerBase
    {
        private IBattleService _battleService;

        public BattleController(IBattleService battleService)
        {
            _battleService = battleService;
        }

        /* TODO
         * This may not be neccessary, possibly endpoints that just get your active battles
         */
        /// <summary>
        /// Get the Battles you're involved in
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the battles</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/Battles")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Battle>))]
        public async Task<IActionResult> GetAll()
        {
            var battles = await _battleService.GetBattlesAsync();

            return Ok(battles);
        }

        /// <summary>
        /// Get a specific battle you're involved in
        /// </summary>
        /// <param name="id">The id of the battle</param>
        /// <returns></returns>
        /// <response code="200">Returns the battle</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Battle not found</response>
        [HttpGet]
        [Route("{id:int}")]
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
        /// <param name="opponent1Id">The id of the character starting the battle (you must own this character)</param>
        /// <param name="opponent2Id">The id of the other character involved in the battle</param>
        /// <returns></returns>
        /// <response code="200">Returns the battle</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="400">Invalid battle</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Character))]
        public async Task<IActionResult> Add([FromQuery] int opponent1Id, [FromQuery] int opponent2Id)
        {
            try
            {
                var battle = await _battleService.AddBattleAsync(opponent1Id, opponent2Id);

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
        /// <param name="battleId">The id of the battle</param>
        /// <param name="opponentId">The id of the other character doing the move</param>
        /// <param name="moveId">The id of the battle move</param>
        /// <returns></returns>
        /// <remarks>
        /// Moves: 1 = Initiate, 2 = Accept, 3 = Attack, 4 = Pursue, 5 = Retreat, 6 = Surrender, 7 = Quit
        /// </remarks>
        /// <response code="200">Returns the battle</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Battle, opponent or move not found</response>
        /// <response code="400">Invalid battle move</response>
        [HttpPost]
        [Route("Move")]
        public async Task<IActionResult> AddMove(int battleId, int opponentId, int moveId)
        {
            try
            {
                var move = (Move)Enum.ToObject(typeof(Move), moveId);

                var results = await _battleService.AddBattleMoveAsync(battleId, opponentId, move);

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
