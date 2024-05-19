using Games.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using System;
using System.Security.Claims;
using System.Xml.Linq;

namespace Games.Services
{
    public interface ICharacterService
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


    public class CharacterService : ICharacterService
    {
        private ICharacterDataService _dataService;

        public const int defaultCharacterCapacity = 3;

        public CharacterService(ICharacterDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IEnumerable<Character>> GetCharactersAsync()
        {
            var characters = await _dataService.GetCharactersAsync();

            return characters;
        }

        public async Task<IEnumerable<Character>> GetDeceasedCharactersAsync()
        {
            var characters = await _dataService.GetDeceasedCharactersAsync();

            return characters;
        }

        public async Task<Character> GetCharacterAsync(int id)
        {
            var character = await _dataService.GetCharacterAsync(id);

            return character;
        }

        public async Task<Character> AddCharacterAsync(string name, int raceId)
        {
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
                    throw new CharacterException("Name already taken");
                }
            }

            var character = await _dataService.GetCharacterAsync(newCharacter.Id);

            return character;
        }

        public async Task<Character> UpdateCharacterNameAsync(int id, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CharacterException("Invalid Name");
            }

            await _dataService.UpdateCharacterNameAsync(id, name);

            var character = await _dataService.GetCharacterAsync(id);

            return character;
        }

        public async Task<Character> UpdateCharacterItemAsync(int id, int itemId, bool primary)
        {
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
                    throw new CharacterException("Invalid item selection"); 
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

            var characterItems = await _dataService.GetCharacterItemsAsync(characterId);

            return characterItems;
        }

        public async Task<CharacterItem> GetCharacterItemAsync(int characterItemId)
        {
            var characterItem = await _dataService.GetCharacterItemAsync(characterItemId);

            return characterItem;
        }

        public async Task<CharacterItem> AddCharacterItemAsync(int characterId, int itemId)
        {
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
                throw new CharacterException("Insufficient capacity");
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
            var battles = await _dataService.GetCharacterBattlesAsync(characterId);

            return battles;
        }
    }
}
