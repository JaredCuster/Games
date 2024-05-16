using Games.Models;
using Games.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Games.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CharacterController : ControllerBase
    {
        private ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        /// <summary>
        /// Get the living characters you own
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the living characters you own</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/Characters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Character>))]
        public async Task<IActionResult> GetAllLiving()
        {
            var characters = await _characterService.GetCharactersAsync();

            return Ok(characters);
        }

        /// <summary>
        /// Get the deceased characters you own
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Returns the deceased characters you own</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/DeceasedCharacters")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Character>))]
        public async Task<IActionResult> GetAllDeceased()
        {
            var characters = await _characterService.GetDeceasedCharactersAsync();

            return Ok(characters);
        }

        /// <summary>
        /// Get a specific character
        /// </summary>
        /// <param name="id">The id of the character you own</param>
        /// <returns></returns>
        /// <response code="200">Returns the character</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Character not found</response>
        [HttpGet]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Character))]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            try
            {
                var character = await _characterService.GetCharacterAsync(id);
                return Ok(character);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Create a character
        /// </summary>
        /// <param name="name">The characters name</param>
        /// <param name="raceId">The characters race</param>
        /// <returns></returns>
        /// <response code="200">Returns the character</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Character))]
        public async Task<IActionResult> Add(string name, int raceId)
        {
            var character = await _characterService.AddCharacterAsync(name, raceId);

            return Ok(character);
        }

        /// <summary>
        /// Update characters name
        /// </summary>
        /// <param name="id">The id of the character you own</param>
        /// <param name="name">The new name of the character</param>
        /// <returns></returns>
        /// <response code="200">Returns the updated character</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Character not found</response>
        /// <response code="400"></response>
        [HttpPut]
        [Route("{id:int}/Name")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Character))]
        public async Task<IActionResult> UpdateName([FromRoute] int id, string name)
        {
            try
            {
                var character = await _characterService.UpdateCharacterNameAsync(id, name);
                return Ok(character);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (CharacterException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update characters primary item
        /// </summary>
        /// <param name="id">The id of the character you own</param>
        /// <param name="inventoryItemId">The item id you have in inventory</param>
        /// <returns></returns>
        /// <response code="200">Returns the updated character</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Character not found</response>
        /// <response code="400"></response>
        [HttpPut]
        [Route("{id:int}/PrimaryItem")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Character))]
        public async Task<IActionResult> UpdatePrimaryItem([FromRoute] int id, int inventoryItemId)
        {
            try
            {
                var character = await _characterService.UpdateCharacterItemAsync(id, inventoryItemId, true);
                return Ok(character);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (CharacterException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Update characters secondary item
        /// </summary>
        /// <param name="id">The id of the character you own</param>
        /// <param name="inventoryItemId">The item id you have in inventory</param>
        /// <returns></returns>
        /// <response code="200">Returns the updated character</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Character not found</response>
        /// <response code="400"></response>
        [HttpPut]
        [Route("{id:int}/SecondaryItem")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Character))]
        public async Task<IActionResult> UpdateSecondaryItem([FromRoute] int id, int inventoryItemId)
        {
            try
            {
                var character = await _characterService.UpdateCharacterItemAsync(id, inventoryItemId, false);
                return Ok(character);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (CharacterException e)
            {
                return BadRequest(e.Message);
            }
        }

        /* TODO
         * Do I want to delete characters, might mess up battles.
         * Maybe just mark them as dead and create end points to only get living, dead or all characters 
         
        /// <summary>
        /// Delete a character
        /// </summary>
        /// <param name="id">The id of the character you own</param>
        /// <returns></returns>
        /// <response code="200">Character deleted</response>
        [HttpDelete]
        [Route("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await _characterService.DeleteCharacterAsync(id);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }*/

        /// <summary>
        /// Get the characters inventory
        /// </summary>
        /// <param name="id">The id of the character you own</param>
        /// <returns></returns>
        /// <response code="200">Returns the characters inventory</response>
        [HttpGet]
        [Route("{id:int}/Inventory")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CharacterItem>))]
        public async Task<IActionResult> GetInventory([FromRoute] int id)
        {
            var items = await _characterService.GetCharacterItemsAsync(id);

            return Ok(items);
        }

        /// <summary>
        /// Get an inventory item
        /// </summary>
        /// <param name="inventoryItemId">The id of the inventory item</param>
        /// <returns></returns>
        /// <response code="200">Returns the inventory item</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Item not found</response>
        [HttpGet]
        [Route("Item/{inventoryItemId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterItem))]
        public async Task<IActionResult> GetItem([FromRoute] int inventoryItemId)
        {
            try
            {
                var item = await _characterService.GetCharacterItemAsync(inventoryItemId);
                return Ok(item);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        /// <summary>
        /// Add an item to the characters inventory
        /// </summary>
        /// <param name="id">The id of the character you own</param>
        /// <param name="itemId">The id of the item</param>
        /// <returns></returns>
        /// <response code="200">Returns the inventory item</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Character or item not found</response>
        /// <response code="400"></response>
        [HttpPost]
        [Route("{id:int}/Item")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterItem))]
        public async Task<IActionResult> AddItem([FromRoute]int id, int itemId)
        {
            try
            {
                var item = await _characterService.AddCharacterItemAsync(id, itemId);
                return Ok(item);
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (CharacterException e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Remove an inventory item
        /// </summary>
        /// <param name="inventoryItemId">The inventory item id</param>
        /// <returns></returns>
        /// <response code="200">Item has been removed from inventory</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Item not found</response>
        [HttpDelete]
        [Route("Item/{inventoryItemId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteItem([FromRoute] int inventoryItemId)
        {
            try
            {
                await _characterService.DeleteCharacterItemAsync(inventoryItemId);
                return Ok();
            }
            catch (NotFoundException e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
