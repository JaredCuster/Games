using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Services
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
        public CharacterDataService(DataContext dataContext, IHttpContextAccessor httpContextAccessor) : 
            base(dataContext, httpContextAccessor)
        {
            
        }

        public async Task<IEnumerable<Character>> GetCharactersAsync()
        {
            var ownerId = GetLoggedInUserId();
            var characters = await _dataContext.Characters
                .Where(c => c.OwnerId == ownerId && c.Health > 0)
                .AsNoTracking()
                .ToListAsync();

            return characters;
        }

        public async Task<IEnumerable<Character>> GetDeceasedCharactersAsync()
        {
            var ownerId = GetLoggedInUserId();
            var characters = await _dataContext.Characters
                .Where(c => c.OwnerId == ownerId && c.Health == 0)
                .AsNoTracking()
                .ToListAsync();

            return characters;
        }

        public async Task<Character> GetCharacterAsync(int id)
        {
            var character = await _dataContext.Characters
                .Where(c => c.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (character == null) { throw new NotFoundException($"Character - ({id}) not found"); }

            return character;
        }

        public async Task AddCharacterAsync(Character dto)
        {
            var ownerId = GetLoggedInUserId();
            dto.OwnerId = ownerId;

            await _dataContext.Characters.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterNameAsync(int id, string name)
        {
            var character = await GetCharacterForUpdateAsync(id);

            character.Name = name;

            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterPrimaryItemAsync(int id, int? itemId)
        {
            var character = await GetCharacterForUpdateAsync(id);

            character.PrimaryItemId = itemId;

            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterSecondaryItemAsync(int id, int? itemId)
        {
            var character = await GetCharacterForUpdateAsync(id);

            character.SecondaryItemId = itemId;

            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteCharacterAsync(int id)
        {
            var character = await _dataContext.Characters.FindAsync(id);
            if (character != null)
            {
                _dataContext.Remove(character);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CharacterItem>> GetCharacterItemsAsync(int characterId)
        {
            var characterItems = await _dataContext.CharacterItems
                .Where(ci => ci.CharacterId == characterId)
                .AsNoTracking()
                .ToListAsync();

            return characterItems;
        }

        public async Task<CharacterItem> GetCharacterItemAsync(int inventoryItemId)
        {
            var characterItem = await _dataContext.CharacterItems
                .Where(ci => ci.Id == inventoryItemId)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (characterItem == null) { throw new NotFoundException("CharacterItem not found"); }

            return characterItem;
        }

        public async Task AddCharacterItemAsync(int characterId, CharacterItem dto)
        {
            await _dataContext.CharacterItems.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
        }

        public async Task DeleteCharacterItemAsync(int inventoryItemId)
        {
            var characterItem = await _dataContext.CharacterItems.FindAsync(inventoryItemId);
            if (characterItem == null) { throw new NotFoundException("CharacterItem not found"); }

            _dataContext.Remove(characterItem);
            await _dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Battle>> GetCharacterBattlesAsync(int characterId)
        {
            var battles = await _dataContext.Battles
                .Where(b => b.Opponent1Id == characterId || b.Opponent1Id == characterId)
                .Where(b => b.Active == true)
                .AsNoTracking()
                .ToListAsync();

            return battles;
        }
    }
}
