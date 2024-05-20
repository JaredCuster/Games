using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Services.DataServices
{
    public interface IBattleDataService : IDataService
    {
        public Task<IEnumerable<Battle>> GetBattlesAsync();

        public Task<Battle> GetBattleAsync(int id);

        public Task<int> AddBattleAsync(Battle dto);

        public Task UpdateBattleLastMoveAsync(int id, int lastMoveId);

        public Task UpdateBattleEndAsync(int id);

        public Task<BattleMove> GetBattleMoveAsync(int id);

        public Task<int> AddBattleMoveAsync(BattleMove dto);

        public Task<Character> GetCharacterAsync(int id);

        public Task UpdateCharacterInBattleAsync(int id, bool inBattle);

        public Task UpdateCharacterHealthAsync(int id, int health);

        public Task<IEnumerable<int>> UpdateCharacterNoInventoryAsync(int id);
    }

    public class BattleDataService : BaseDataService, IBattleDataService
    {
        public BattleDataService(ILogger<IBattleDataService> logger, DataContext dataContext, IHttpContextAccessor httpContextAccessor) :
            base(logger, dataContext, httpContextAccessor)
        {

        }

        public async Task<IEnumerable<Battle>> GetBattlesAsync()
        {
            _logger.LogDebug($"{nameof(GetBattlesAsync)}");

            var battles = await _dataContext.Battles
                .AsNoTracking()
                .ToListAsync();

            return battles;
        }

        public async Task<Battle> GetBattleAsync(int id)
        {
            _logger.LogDebug($"{nameof(GetBattleAsync)} - {id}");

            var battle = await _dataContext.Battles
                .Where(b => b.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (battle == null) 
            {
                var msg = $"Battle ({id}) not found";
                _logger.LogWarning($"{nameof(GetBattleAsync)} {msg}");
                throw new NotFoundException(msg); 
            }

            return battle;
        }

        public async Task<int> AddBattleAsync(Battle dto)
        {
            _logger.LogDebug($"{nameof(AddBattleAsync)} - {dto}");

            await _dataContext.Battles.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
            return dto.Id;
        }

        public async Task UpdateBattleEndAsync(int id)
        {
            _logger.LogDebug($"{nameof(UpdateBattleEndAsync)} - {id}");

            var battle = await GetBattleForUpdateAsync(id);

            battle.Active = false;
            battle.EndDate = DateTime.UtcNow;

            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateBattleLastMoveAsync(int id, int lastMoveId)
        {
            _logger.LogDebug($"{nameof(UpdateBattleLastMoveAsync)} - {id}:{lastMoveId}");

            var battle = await GetBattleForUpdateAsync(id);

            battle.LastMoveId = lastMoveId;

            await _dataContext.SaveChangesAsync();
        }

        public async Task<BattleMove> GetBattleMoveAsync(int id)
        {
            _logger.LogDebug($"{nameof(GetBattleMoveAsync)} - {id}");

            var battleMove = await _dataContext.BattleMoves
                .FirstOrDefaultAsync(bm => bm.Id == id);

            if (battleMove == null) 
            {
                var msg = $"Move ({id}) not found";
                _logger.LogWarning($"{nameof(GetBattleMoveAsync)} {msg}");
                throw new NotFoundException(msg); 
            }

            return battleMove;
        }

        public async Task<int> AddBattleMoveAsync(BattleMove dto)
        {
            _logger.LogDebug($"{nameof(AddBattleMoveAsync)} - {dto}");

            await _dataContext.BattleMoves.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
            return dto.Id;
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

        public async Task UpdateCharacterInBattleAsync(int id, bool inBattle)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterInBattleAsync)} - {id}:{inBattle}");

            var character = await GetCharacterForUpdateAsync(id);

            character.InBattle = inBattle;

            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterHealthAsync(int id, int health)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterHealthAsync)} - {id}:{health}");

            var character = await GetCharacterForUpdateAsync(id);

            character.Health = health;

            await _dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<int>> UpdateCharacterNoInventoryAsync(int id)
        {
            _logger.LogDebug($"{nameof(UpdateCharacterNoInventoryAsync)} - {id}");

            var itemIds = new List<int>();

            var inventory = await _dataContext.CharacterItems
                    .Where(ci => ci.CharacterId == id).ToListAsync();
            foreach (var item in inventory)
            {
                itemIds.Add(item.ItemId);
                _dataContext.CharacterItems.Remove(item);
            }
            await _dataContext.SaveChangesAsync();

            return itemIds;
        }
    }
}
