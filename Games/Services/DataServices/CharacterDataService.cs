using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Services.DataServices
{
    public interface ICharacterDataService : IDataService
    {
        public Task<IEnumerable<Character>> GetCharactersAsync();

        public Task<IEnumerable<Character>> GetDeceasedCharactersAsync();

        public Task<Character> GetCharacterAsync(int id);

        public Task AddCharacterAsync(Character dto);

        public Task UpdateCharacterNameAsync(int id, string name);

        public Task UpdateCharacterPrimaryItemAsync(int id, int? itemId);

        public Task UpdateCharacterSecondaryItemAsync(int id, int? itemId);

        public Task DeleteCharacterAsync(int id);


        public Task<IEnumerable<CharacterItem>> GetCharacterItemsAsync(int characterId);

        public Task<CharacterItem> GetCharacterItemAsync(int inventoryItemId);

        public Task AddCharacterItemAsync(int characterId, CharacterItem characterItem);

        public Task DeleteCharacterItemAsync(int inventoryItemId);

        public Task<IEnumerable<Battle>> GetCharacterBattlesAsync(int characterId);
    }

    public class CharacterDataService : BaseDataService, ICharacterDataService
    {
        public CharacterDataService(ILogger<ICharacterDataService> logger, DataContext dataContext, IHttpContextAccessor httpContextAccessor) :
            base(logger, dataContext, httpContextAccessor)
        {

        }

        public async Task<IEnumerable<Character>> GetCharactersAsync()
        {
            _logger.LogDebug($"{nameof(GetCharactersAsync)}");

            var ownerId = GetLoggedInUserId();
            var characters = await _dataContext.Characters
                .Where(c => c.OwnerId == ownerId && c.Health > 0)
                .AsNoTracking()
                .ToListAsync();

            return characters;
        }

        public async Task<IEnumerable<Character>> GetDeceasedCharactersAsync()
        {
            _logger.LogDebug($"{nameof(GetDeceasedCharactersAsync)}");

            var ownerId = GetLoggedInUserId();
            var characters = await _dataContext.Characters
                .Where(c => c.OwnerId == ownerId && c.Health == 0)
                .AsNoTracking()
                .ToListAsync();

            return characters;
        }

        public async Task<Character> GetCharacterAsync(int id)
        {
            _logger.LogDebug($"{nameof(GetCharacterAsync)} - {id}");

            var character = await _dataContext.Characters
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (character == null) 
            {
                var msg = $"Character ({id}) not found";
                _logger.LogWarning($"{nameof(GetCharacterAsync)} {msg}");
                throw new NotFoundException(msg);
            }

            return character;
        }

        public async Task AddCharacterAsync(Character dto)
        {
            _logger.LogDebug($"{nameof(AddCharacterAsync)} - {dto}");

            var ownerId = GetLoggedInUserId();
            dto.OwnerId = ownerId;

            await _dataContext.Characters.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterNameAsync(int id, string name)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterNameAsync)} - {id}:{name}");

            var character = await GetCharacterForUpdateAsync(id);

            character.Name = name;

            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterPrimaryItemAsync(int id, int? itemId)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterPrimaryItemAsync)} - {id}:{itemId}");

            var character = await GetCharacterForUpdateAsync(id);

            character.PrimaryItemId = itemId;

            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterSecondaryItemAsync(int id, int? itemId)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterSecondaryItemAsync)} - {id}:{itemId}");

            var character = await GetCharacterForUpdateAsync(id);

            character.SecondaryItemId = itemId;

            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteCharacterAsync(int id)
        {
            _logger.LogDebug($"{nameof(DeleteCharacterAsync)} - {id}");

            var character = await _dataContext.Characters.FindAsync(id);
            if (character != null)
            {
                _dataContext.Remove(character);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CharacterItem>> GetCharacterItemsAsync(int characterId)
        {
            _logger.LogDebug($"{nameof(GetCharacterItemsAsync)} - {characterId}");

            var characterItems = await _dataContext.CharacterItems
                .Where(ci => ci.CharacterId == characterId)
                .AsNoTracking()
                .ToListAsync();

            return characterItems;
        }

        public async Task<CharacterItem> GetCharacterItemAsync(int inventoryItemId)
        {
            _logger.LogDebug($"{nameof(GetCharacterItemAsync)} - {inventoryItemId}");

            var characterItem = await _dataContext.CharacterItems
                .Where(ci => ci.Id == inventoryItemId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (characterItem == null) 
            {
                var msg = $"Inventory Item ({inventoryItemId}) not found";
                _logger.LogWarning($"{nameof(GetCharacterItemAsync)} {msg}");
                throw new NotFoundException(msg);
            }

            return characterItem;
        }

        public async Task AddCharacterItemAsync(int characterId, CharacterItem dto)
        {
            _logger.LogDebug($"{nameof(AddCharacterItemAsync)} - {characterId}:{dto}");

            await _dataContext.CharacterItems.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteCharacterItemAsync(int inventoryItemId)
        {
            _logger.LogDebug($"{nameof(DeleteCharacterItemAsync)} - {inventoryItemId}");

            var characterItem = await _dataContext.CharacterItems.FindAsync(inventoryItemId);
            if (characterItem == null) 
            {
                var msg = $"Inventory Item ({inventoryItemId}) not found";
                _logger.LogWarning($"{nameof(DeleteCharacterItemAsync)} {msg}");
                throw new NotFoundException(msg);
            }

            _dataContext.Remove(characterItem);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Battle>> GetCharacterBattlesAsync(int characterId)
        {
            _logger.LogDebug($"{nameof(GetCharacterBattlesAsync)} - {characterId}");

            var battles = await _dataContext.Battles
                .Where(b => b.Opponent1Id == characterId || b.Opponent1Id == characterId)
                .Where(b => b.Active == true)
                .AsNoTracking()
                .ToListAsync();

            return battles;
        }
    }
}
