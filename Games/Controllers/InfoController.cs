using Games.Models;
using Games.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class InfoController : ControllerBase
    {
        private IInfoService _infoService;

        public InfoController(IInfoService infoService)
        {
            _infoService = infoService;
        }

        /*/// <summary>
        /// Gets the character skills
        /// </summary>
        /// <returns>The character skills</returns>
        /// <response code="200">Returns the skills</response>
        [HttpGet]
        [Route("Skills")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CharacterSkill>))]
        public IActionResult GetSkills()
        {
            var values = new List<object>();
            foreach (int i in Enum.GetValues(typeof(CharacterSkill)))
            {
                values.Add(new
                {
                    Id = i,
                    Name = Enum.GetName(typeof(CharacterSkill), i)
                });
            }

            return Ok(values);
        }*/

        /// <summary>
        /// Get the character races
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Skills: 1 = None, 2 = Strength, 3 = Intelligence, 4 = Stealth
        /// </remarks>
        /// <response code="200">Retuns the skills</response>
        [HttpGet]
        [Route("Races")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CharacterRace>))]
        public async Task<IActionResult> GetRaces()
        {
            var values = await _infoService.GetRacesAsync();

            return Ok(values);
        }

        /// <summary>
        /// Get the items a character can have in inventory
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Categories: 0 = None, 1 = Offense, 2 = Defense, 3 = Healing
        /// </remarks>
        /// <response code="200">Retuns the items</response>
        [HttpGet]
        [Route("Items")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Item>))]
        public async Task<IActionResult> GetItems()
        {
            var values = await _infoService.GetItemsAsync();

            return Ok(values);
        }

        /*/// <summary>
        /// Gets the moves a character can make in battle
        /// </summary>
        /// <returns>Returns the moves a character can make in battle</returns>
        /// <response code="200">Retuns the moves</response>
        [HttpGet]
        [Route("Moves")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Move>))]
        public IActionResult GetBattleMoves()
        {
            var values = new List<object>();
            foreach (int i in Enum.GetValues(typeof(Move)))
            {
                values.Add(new {
                    Id = i,
                    Name = Enum.GetName(typeof(Move), i)
                });
            }

            return Ok(values);
        }*/
    }
}
