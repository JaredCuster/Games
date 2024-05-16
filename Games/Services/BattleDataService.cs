using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Services
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
        public BattleDataService(DataContext dataContext, IHttpContextAccessor httpContextAccessor) : 
            base(dataContext, httpContextAccessor)
        {
            
        }

        public async Task<IEnumerable<Battle>> GetBattlesAsync()
        {
            var battles = await _dataContext.Battles
                .AsNoTracking()
                .ToListAsync();

            return battles;
        }

        public async Task<Battle> GetBattleAsync(int id)
        {
            var battle = await _dataContext.Battles
                .Where(b => b.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (battle == null) { throw new NotFoundException($"Battle - ({id}) not found"); }

            return battle;
        }

        public async Task<int> AddBattleAsync(Battle dto)
        {
            await _dataContext.Battles.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
            return dto.Id;
        }

        public async Task UpdateBattleEndAsync(int id)
        {
            var battle = await GetBattleForUpdateAsync(id);

            battle.Active = false;
            battle.EndDate = DateTime.UtcNow;

            var opponent1 = await GetCharacterForUpdateAsync(battle.Opponent1Id);
            opponent1.InBattle = false;

            var opponent2 = await GetCharacterForUpdateAsync(battle.Opponent2Id);
            opponent2.InBattle = false;

            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateBattleLastMoveAsync(int id, int lastMoveId)
        {
            var battle = await GetBattleForUpdateAsync(id);

            battle.LastMoveId = lastMoveId;
            
            await _dataContext.SaveChangesAsync();
        }

        public async Task<BattleMove> GetBattleMoveAsync(int id)
        {
            var battleMove = await _dataContext.BattleMoves
                .FirstOrDefaultAsync(bm => bm.Id == id);

            if (battleMove == null) { throw new NotFoundException($"Move - ({id}) not found"); }

            return battleMove;
        }

        public async Task<int> AddBattleMoveAsync(BattleMove dto)
        {
            await _dataContext.BattleMoves.AddAsync(dto);
            await _dataContext.SaveChangesAsync();
            return dto.Id;
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

        public async Task UpdateCharacterInBattleAsync(int id, bool inBattle)
        {
            var character = await GetCharacterForUpdateAsync(id);

            character.InBattle = inBattle;
            await _dataContext.SaveChangesAsync();
        }

        public async Task UpdateCharacterHealthAsync(int id, int health)
        {
            var character = await GetCharacterForUpdateAsync(id);

            character.Health = health;

            await _dataContext.SaveChangesAsync();
        }

        /*
         * TODO return items for use by the winner
         * see Update
         */
        public async Task<IEnumerable<int>> UpdateCharacterNoInventoryAsync(int id)
        {
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
