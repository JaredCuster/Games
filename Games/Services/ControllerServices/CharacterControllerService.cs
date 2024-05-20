using Games.Models;
using Games.Services.DataServices;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System;
using System.Security.Claims;
using System.Xml.Linq;

namespace Games.Services.ControllerServices
{
    public interface ICharacterControllerService
    {
        public Task<IEnumerable<Character>> GetCharactersAsync();

        public Task<IEnumerable<Character>> GetDeceasedCharactersAsync();

        public Task<Character> GetCharacterAsync(int id);

        public Task<Character> AddCharacterAsync(string name, int raceId);

        public Task<Character> UpdateCharacterNameAsync(int id, string name);

        public Task<Character> UpdateCharacterItemAsync(int id, int itemId, bool primary);

        public Task DeleteCharacterAsync(int id);


        public Task<IEnumerable<CharacterItem>> GetCharacterItemsAsync(int characterId);

        public Task<CharacterItem> GetCharacterItemAsync(int characterItemId);

        public Task<CharacterItem> AddCharacterItemAsync(int characterId, int itemId);

        public Task DeleteCharacterItemAsync(int characterItemId);

        public Task<IEnumerable<Battle>> GetCharacterBattlesAsync(int characterId);
    }


    public class CharacterControllerService : ICharacterControllerService
    {
        private readonly ILogger<ICharacterControllerService> _logger;
        private readonly ICharacterDataService _dataService;

        public const int defaultCharacterCapacity = 3;

        public CharacterControllerService(ILogger<ICharacterControllerService> logger, ICharacterDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<IEnumerable<Character>> GetCharactersAsync()
        {
            _logger.LogDebug($"{nameof(GetCharactersAsync)}");

            var characters = await _dataService.GetCharactersAsync();

            return characters;
        }

        public async Task<IEnumerable<Character>> GetDeceasedCharactersAsync()
        {
            _logger.LogDebug($"{nameof(GetDeceasedCharactersAsync)}");

            var characters = await _dataService.GetDeceasedCharactersAsync();

            return characters;
        }

        public async Task<Character> GetCharacterAsync(int id)
        {
            _logger.LogDebug($"{nameof(GetCharacterAsync)} - {id}");

            var character = await _dataService.GetCharacterAsync(id);

            return character;
        }

        public async Task<Character> AddCharacterAsync(string name, int raceId)
        {
            _logger.LogDebug($"{nameof(AddCharacterAsync)} - {name}:{raceId}");

            var newCharacter = new Character()
            {
                Name = name,
                RaceId = raceId,
                Health = 100,
                Level = 1,
                InBattle = false,
                CreateDate = DateTime.Now
            };

            try
            {
                await _dataService.AddCharacterAsync(newCharacter);
            }
            catch (DbUpdateException dbe)
            {
                var ie = dbe.InnerException as SqliteException;
                if (ie?.SqliteErrorCode == 19)
                {
                    var msg = "Name already taken";
                    _logger.LogWarning($"{nameof(AddCharacterAsync)} {msg}");
                    throw new CharacterException(msg);
                }
            }

            var character = await _dataService.GetCharacterAsync(newCharacter.Id);

            return character;
        }

        public async Task<Character> UpdateCharacterNameAsync(int id, string name)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterNameAsync)} -  {id}:{name}");

            if (string.IsNullOrWhiteSpace(name))
            {
                var msg = "Invalid Name";
                _logger.LogWarning($"{nameof(UpdateCharacterNameAsync)} {msg}");
                throw new CharacterException(msg);
            }

            await _dataService.UpdateCharacterNameAsync(id, name);

            var character = await _dataService.GetCharacterAsync(id);

            return character;
        }

        public async Task<Character> UpdateCharacterItemAsync(int id, int itemId, bool primary)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterItemAsync)} - {id}:{itemId}:{primary}");

            if (itemId == 0)
            {
                if (primary)
                {
                    await _dataService.UpdateCharacterPrimaryItemAsync(id, null);
                }
                else
                {
                    await _dataService.UpdateCharacterSecondaryItemAsync(id, null);
                }
            }
            else
            {
                if (!await IsItemUsable(id, itemId, primary))
                {
                    var msg = "Invalid item selection";
                    _logger.LogWarning($"{nameof(UpdateCharacterItemAsync)} {msg}");
                    throw new CharacterException(msg);
                }

                if (primary)
                {
                    await _dataService.UpdateCharacterPrimaryItemAsync(id, itemId);
                }
                else
                {
                    await _dataService.UpdateCharacterSecondaryItemAsync(id, itemId);
                }
            }

            var character = await _dataService.GetCharacterAsync(id);

            return character;
        }

        private async Task<bool> IsItemUsable(int id, int itemId, bool primary)
        {
            var character = await _dataService.GetCharacterAsync(id);
            var inventoryItem = character.Inventory.FirstOrDefault(ci => ci.Id == itemId);

            if (inventoryItem == null) return false;

            if (primary && character.PrimaryItemId == itemId) return false;

            if (!primary && character.SecondaryItemId == itemId) return false;

            return true;
        }

        public async Task DeleteCharacterAsync(int id)
        {
            _logger.LogDebug($"{nameof(DeleteCharacterAsync)} - {id}");

            await _dataService.DeleteCharacterAsync(id);
        }


        public async Task<IEnumerable<CharacterItem>> GetCharacterItemsAsync(int characterId)
        {
            /*var items = await _dataContext.Database
                .SqlQuery<CharactersItem>(
                    @$"SELECT i.Name AS Name, i.Value AS Value, i.Capacity AS Capacity, ic.Category AS Category 
                       FROM CharacterItems AS ci
                       INNER JOIN Items AS i on ci.ItemId = i.Id
                       INNER JOIN ItemCategories AS ic ON i.CategoryId = ic.Id
                       WHERE ci.CharacterId = {characterId}")
                .ToListAsync();*/
            _logger.LogDebug($"{nameof(GetCharacterItemsAsync)} - {characterId}");

            var characterItems = await _dataService.GetCharacterItemsAsync(characterId);

            return characterItems;
        }

        public async Task<CharacterItem> GetCharacterItemAsync(int characterItemId)
        {
            _logger.LogDebug($"{nameof(GetCharacterItemAsync)} - {characterItemId}");

            var characterItem = await _dataService.GetCharacterItemAsync(characterItemId);

            return characterItem;
        }

        public async Task<CharacterItem> AddCharacterItemAsync(int characterId, int itemId)
        {
            _logger.LogDebug($"{nameof(AddCharacterItemAsync)} - {characterId}:{itemId}");

            var items = await _dataService.GetCharacterItemsAsync(characterId);
            var capacity = defaultCharacterCapacity;
            foreach (var item in items)
            {
                if (item.Item != null)
                {
                    capacity += item.Item.Capacity;
                }
            }

            if (capacity <= items.Count())
            {
                var msg = "Insufficient capacity";
                _logger.LogWarning($"{nameof(UpdateCharacterItemAsync)} {msg}");
                throw new CharacterException(msg);
            }

            var newCharacterItem = new CharacterItem()
            {
                CharacterId = characterId,
                ItemId = itemId,
            };

            await _dataService.AddCharacterItemAsync(characterId, newCharacterItem);

            var characterItem = await _dataService.GetCharacterItemAsync(newCharacterItem.Id);

            return characterItem;
        }

        public async Task DeleteCharacterItemAsync(int characterItemId)
        {
            _logger.LogDebug($"{nameof(DeleteCharacterItemAsync)} - :{characterItemId}");

            using var transaction = _dataService.BeginTransaction();

            var characterItem = await _dataService.GetCharacterItemAsync(characterItemId);

            var character = await _dataService.GetCharacterAsync(characterItem.CharacterId);
            if (character.PrimaryItemId == characterItemId)
            {
                await _dataService.UpdateCharacterPrimaryItemAsync(character.Id, null);
            }
            if (character.SecondaryItemId == characterItemId)
            {
                await _dataService.UpdateCharacterSecondaryItemAsync(character.Id, null);
            }

            await _dataService.DeleteCharacterItemAsync(characterItemId);

            _dataService.CommitTransaction(transaction);
        }

        public async Task<IEnumerable<Battle>> GetCharacterBattlesAsync(int characterId)
        {
            _logger.LogDebug($"{nameof(GetCharacterBattlesAsync)} - {characterId}");

            var battles = await _dataService.GetCharacterBattlesAsync(characterId);

            return battles;
        }
    }
}
