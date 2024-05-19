using Games.Models;

namespace Games.Services
{
    public interface IBattleService
    {
        public Task<IEnumerable<Battle>> GetBattlesAsync();

        public Task<Battle> GetBattleAsync(int id);

        public Task<Battle> AddBattleAsync(int opponent1Id, int opponent2Id);

        public Task<BattleMoveResults> AddBattleMoveAsync(int battleId, int opponentId, string moveName);
    }

    public class BattleService : IBattleService
    {
        private IBattleDataService _dataService;

        public BattleService(IBattleDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IEnumerable<Battle>> GetBattlesAsync()
        {
            var battles = await _dataService.GetBattlesAsync();

            return battles;
        }

        public async Task<Battle> GetBattleAsync(int id)
        {
            var battle = await _dataService.GetBattleAsync(id);

            return battle;
        }

        public async Task<Battle> AddBattleAsync(int opponent1Id, int opponent2Id)
        {
            using var transaction = _dataService.BeginTransaction();

            var opponent1 = await _dataService.GetCharacterAsync(opponent1Id);
            var opponent2 = await _dataService.GetCharacterAsync(opponent2Id);
            if (opponent1.InBattle || opponent2.InBattle)
            {
                throw new BattleException("One of the characters are already in battle");
            }

            var newBattle = new Battle()
            {
                Opponent1Id = opponent1Id,
                Opponent2Id = opponent2Id,
                Active = true,
                StartDate = DateTime.Now
            };
            var battleId = await _dataService.AddBattleAsync(newBattle);

            var newBattleMove = new BattleMove()
            {
                BattleId = battleId,
                OpponentId = opponent1Id,
                Move = Move.Initiate,
                CreateDate = DateTime.Now
            };
            var battleMoveId = await _dataService.AddBattleMoveAsync(newBattleMove);

            await _dataService.UpdateBattleLastMoveAsync(battleId, battleMoveId);

            await _dataService.UpdateCharacterInBattleAsync(opponent1.Id, true);
            await _dataService.UpdateCharacterInBattleAsync(opponent2.Id, true);

            var battle = await GetBattleAsync(battleId);

            _dataService.CommitTransaction(transaction);

            return battle;
        }

        public async Task<BattleMoveResults> AddBattleMoveAsync(int battleId, int opponentId, string moveName)
        {
            // Parse the move
            Move move;
            try
            {
                move = Enum.Parse<Move>(moveName, true);
            }
            catch
            {
                var moves = string.Join(",", Enum.GetValues<Move>().Select(m => Enum.GetName(m)).ToList());
                throw new BattleException($"Invalid Move - {moveName} Valid Moves: {moves}");
            }

            using var transaction = _dataService.BeginTransaction();

            // Validate move
            var battle = await _dataService.GetBattleAsync(battleId);
            if (battle.LastMove?.OpponentId == opponentId)
            {
                throw new BattleException("Its not your turn");
            }
            ValidateMove(battle, move);

            // Execute the move
            var aggressor = await _dataService.GetCharacterAsync(opponentId);
            var defenderId = opponentId != battle.Opponent1Id ? battle.Opponent1Id : battle.Opponent2Id;
            var defender = await _dataService.GetCharacterAsync(defenderId);
            var results = await ExecuteMoveAsync(move, battle, aggressor, defender);

            // Save the move
            var newBattleMove = new BattleMove()
            {
                BattleId = battleId,
                OpponentId = opponentId,
                Move = move,
                CreateDate = DateTime.Now
            };
            var moveId = await _dataService.AddBattleMoveAsync(newBattleMove);
            await _dataService.UpdateBattleLastMoveAsync(battleId, moveId);
            results.BattleMove = newBattleMove;

            _dataService.CommitTransaction(transaction);

            return results;
        }

        private void ValidateMove(Battle battle, Move move)
        {
            var lastMove = battle.LastMove?.Move;
            switch (lastMove)
            {
                case Move.Initiate:
                    if (move == Move.Accept || move == Move.Retreat)
                    {
                        break;
                    }
                    throw new BattleException("Invalid Move - You can only Accept or Retreat");

                case Move.Accept:
                    if (move == Move.Attack || move == Move.Surrender)
                    {
                        break;
                    }
                    throw new BattleException("Invalid Move - You can only Attack or Surrender");

                case Move.Attack:
                case Move.Pursue:
                    if (move == Move.Attack || move == Move.Retreat || move == Move.Surrender)
                    {
                        break; 
                    }
                    throw new BattleException("Invalid Move - You can only Attack, Retreat or Surrender");

                case Move.Retreat:
                    if (move == Move.Pursue || move == Move.Quit)
                    {
                        break; 
                    }
                    throw new BattleException("Invalid Move - You can only Pursue or Quit");

                case Move.Surrender:
                    // finsish the battle with spoils
                    break;

                case Move.Quit:
                    // finish the battle with no spoils
                    break;

                default:
                    throw new BattleException("Invalid Move - No Move Specified");
            }
        }

        private async Task<BattleMoveResults> ExecuteMoveAsync(Move move, Battle battle, Character aggressor, Character defender)
        {
            switch (move)
            {
                case Move.Attack:
                    return await ExecuteAttackAsync(battle, aggressor, defender);

                case Move.Surrender:
                    return await ExecuteSurrenderAsync(battle, aggressor, defender);

                case Move.Quit:
                    return await ExecuteQuitAsync(battle, aggressor, defender);

                case Move.Pursue:
                case Move.Retreat:
                case Move.Initiate:
                case Move.Accept:
                    return new BattleMoveResults();
                
                default:
                    throw new BattleException("Invalid Move - No Move Specified");
            }
        }

        private async Task<BattleMoveResults> ExecuteAttackAsync(Battle battle, Character aggressor, Character defender) 
        {
            var results = new BattleMoveResults();

            // Calculate attack damage
            var aggressorValue = CalculateMoveValue(aggressor, ItemCategory.Offense);
            var defenderValue = CalculateMoveValue(defender, ItemCategory.Defense);
            var damage = aggressorValue - defenderValue;
            results.Damage = damage;

            // Attack caused damage
            if (damage > 0)
            {
                var result = defender.Health - damage;
                if (result <= 0)
                {
                    // Defender Died
                    await _dataService.UpdateCharacterHealthAsync(defender.Id, 0);
                    results.Plunder = await _dataService.UpdateCharacterNoInventoryAsync(defender.Id);
                    
                    results.BattleIsOver = await EndBattle(battle.Id, aggressor.Id, defender.Id);
                }
                else
                {
                    defender.Health = result;
                    await _dataService.UpdateCharacterHealthAsync(defender.Id, result);
                }
            }

            return results;
        }

        private async Task<BattleMoveResults> ExecuteSurrenderAsync(Battle battle, Character aggressor, Character defender)
        {
            var results = new BattleMoveResults();

            // Defender Surrenders
            var health = defender.Health == 1 ? defender.Health : defender.Health / 2;
            await _dataService.UpdateCharacterHealthAsync(defender.Id, health);
            results.Plunder = await _dataService.UpdateCharacterNoInventoryAsync(defender.Id);

            results.BattleIsOver = await EndBattle(battle.Id, aggressor.Id, defender.Id);

            return results;
        }

        private async Task<BattleMoveResults> ExecuteQuitAsync(Battle battle, Character aggressor, Character defender)
        {
            var results = new BattleMoveResults();

            results.BattleIsOver = await EndBattle(battle.Id, aggressor.Id, defender.Id);

            return results;
        }

        private async Task<bool> EndBattle(int battleId, int aggressorId, int defenderId)
        {
            await _dataService.UpdateBattleEndAsync(battleId);
            await _dataService.UpdateCharacterInBattleAsync(aggressorId, false);
            await _dataService.UpdateCharacterInBattleAsync(defenderId, false);
            return true;
        }

        private int CalculateMoveValue(Character character, ItemCategory category)
        {
            var characterValue = GetCharacterValue(character, category);
            var primaryItemValue = GetItemValue(character.PrimaryItem, category);
            var secondaryItemValue = GetItemValue(character.SecondaryItem, category);
            var totalValue = characterValue + primaryItemValue + secondaryItemValue;

            var dice = new Random();
            var diceValue = dice.Next(1, 6);

            var moveValue = totalValue * diceValue;
            return moveValue;
        }

        private int GetCharacterValue(Character character, ItemCategory category)
        {
            var skill = character.Race?.Skill;
            var level = character.Level;

            switch (category)
            {
                case ItemCategory.Offense:
                    switch (skill)
                    {
                        case CharacterSkill.Strength:
                            return 10 + level;
                        case CharacterSkill.Intelligence:
                            return 5 + level;
                        default:
                            return level;
                    }

                case ItemCategory.Defense:
                    switch (skill)
                    {
                        case CharacterSkill.Stealth:
                            return 10 + level;
                        case CharacterSkill.Intelligence:
                            return 5 + level;
                        default:
                            return level;
                    }

                default:
                    return level;
            }
        }

        private int GetItemValue(CharacterItem? item, ItemCategory category)
        {
            var itemValueNullable = item?.Item?.Category == category ? item.Item?.Value : 0;
            var itemValue = itemValueNullable == null ? 0 : (int)itemValueNullable;
            return itemValue;
        }
    }
}
