using Games.Models;
using Games.Services.DataServices;
using Games.Services.MessageServices;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace Games.Services.ControllerServices
{
    public interface IBattleControllerService
    {
        public Task<IEnumerable<Battle>> GetBattlesAsync();

        public Task<Battle> GetBattleAsync(int id);

        public Task<Battle> AddBattleAsync(int opponent1Id, int opponent2Id);

        public Task<BattleMoveResults> AddBattleMoveAsync(int battleId, int opponentId, string moveName);
    }

    public class BattleControllerService : IBattleControllerService
    {
        private readonly ILogger<IBattleControllerService> _logger;
        private readonly IMessageService _messageService;
        private readonly IBattleDataService _dataService;

        public BattleControllerService(ILogger<IBattleControllerService> logger, IMessageService messageService, IBattleDataService dataService)
        {
            _logger = logger;
            _messageService = messageService;
            _dataService = dataService;
        }

        public async Task<IEnumerable<Battle>> GetBattlesAsync()
        {
            _logger.LogDebug($"{nameof(GetBattlesAsync)}");

            var battles = await _dataService.GetBattlesAsync();

            return battles;
        }

        public async Task<Battle> GetBattleAsync(int id)
        {
            _logger.LogDebug($"{nameof(GetBattleAsync)} - {id}");

            var battle = await _dataService.GetBattleAsync(id);

            return battle;
        }

        public async Task<Battle> AddBattleAsync(int opponent1Id, int opponent2Id)
        {
            _logger.LogDebug($"{nameof(AddBattleAsync)} - {opponent1Id}:{opponent2Id}");

            using var transaction = _dataService.BeginTransaction();

            var opponent1 = await _dataService.GetCharacterAsync(opponent1Id);
            var opponent2 = await _dataService.GetCharacterAsync(opponent2Id);
            if (opponent1.InBattle || opponent2.InBattle)
            {
                var opponentMsg = "One of the opponents";
                if (opponent1.InBattle) { opponentMsg = "Opponent 1"; }
                else if (opponent2.InBattle) { opponentMsg = "Opponent 1"; }
                var msg = $"{opponentMsg} is already in battle";
                
                _logger.LogWarning($"{nameof(AddBattleMoveAsync)} {msg}");
                throw new BattleException(msg);
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

            _logger.LogInformation($"{nameof(AddBattleAsync)} - Battle started between {opponent1.Name} and {opponent2.Name}");

            return battle;
        }

        public async Task<BattleMoveResults> AddBattleMoveAsync(int battleId, int opponentId, string moveName)
        {
            _logger.LogDebug($"{nameof(AddBattleMoveAsync)} - {battleId}:{opponentId}:{moveName}");

            // Parse the move
            Move move;
            try
            {
                move = Enum.Parse<Move>(moveName, true);
            }
            catch
            {
                var moves = string.Join(",", Enum.GetValues<Move>().Select(m => Enum.GetName(m)).ToList());
                var msg = $"Invalid Move - {moveName} Valid Moves: {moves}";
                _logger.LogWarning($"{nameof(AddBattleMoveAsync)} {msg}");
                throw new BattleException(msg);
            }

            using var transaction = _dataService.BeginTransaction();

            // Validate move
            var battle = await _dataService.GetBattleAsync(battleId);
            if (battle.LastMove?.OpponentId == opponentId)
            {
                var msg = "Its not your turn";
                _logger.LogWarning($"{nameof(AddBattleMoveAsync)} {msg}");
                throw new BattleException(msg);
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

            _logger.LogInformation($"{nameof(AddBattleMoveAsync)} - {aggressor.Name} {results.BattleMove.Move} => {defender.Name}, Damage:{results.Damage}");

            await _messageService.SendMessage(JsonSerializer.Serialize(results));

            return results;
        }

        private void ValidateMove(Battle battle, Move move)
        {
            var lastMove = battle.LastMove?.Move;
            _logger.LogDebug($"{nameof(ValidateMove)} - {lastMove}:{move}");
            switch (lastMove)
            {
                case Move.Initiate:
                    if (move == Move.Accept || move == Move.Retreat)
                    {
                        break;
                    }
                    var msg = "Invalid Move - You can only Accept or Retreat";
                    _logger.LogWarning($"{nameof(ValidateMove)} {msg}");
                    throw new BattleException(msg);

                case Move.Accept:
                    if (move == Move.Attack || move == Move.Surrender)
                    {
                        break;
                    }
                    msg = "Invalid Move - You can only Attack or Surrender";
                    _logger.LogWarning($"{nameof(ValidateMove)} {msg}");
                    throw new BattleException(msg);

                case Move.Attack:
                case Move.Pursue:
                    if (move == Move.Attack || move == Move.Retreat || move == Move.Surrender)
                    {
                        break;
                    }
                    msg = "Invalid Move - You can only Attack, Retreat or Surrender";
                    _logger.LogWarning($"{nameof(ValidateMove)} {msg}");
                    throw new BattleException(msg);

                case Move.Retreat:
                    if (move == Move.Pursue || move == Move.Quit)
                    {
                        break;
                    }
                    msg = "Invalid Move - You can only Pursue or Quit";
                    _logger.LogWarning($"{nameof(ValidateMove)} {msg}");
                    throw new BattleException(msg);

                case Move.Surrender:
                    // TODO finsish the battle with spoils
                    break;

                case Move.Quit:
                    // TODO finish the battle with no spoils
                    break;

                default:
                    msg = "Invalid Move - No Move Specified";
                    _logger.LogWarning($"{nameof(ValidateMove)} {msg}");
                    throw new BattleException(msg);
            }
        }

        private async Task<BattleMoveResults> ExecuteMoveAsync(Move move, Battle battle, Character aggressor, Character defender)
        {
            _logger.LogDebug($"{nameof(ExecuteMoveAsync)} - {move}:{battle.Id}:{aggressor.Name}:{defender.Name}");

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
                    var msg = "Invalid Move - No Move Specified";
                    _logger.LogWarning($"{nameof(ExecuteMoveAsync)} {msg}");
                    throw new BattleException(msg);
            }
        }

        private async Task<BattleMoveResults> ExecuteAttackAsync(Battle battle, Character aggressor, Character defender)
        {
            _logger.LogDebug($"{nameof(ExecuteAttackAsync)} - {battle.Id}:{aggressor.Name}:{defender.Name}");

            var results = new BattleMoveResults();

            // Calculate attack damage
            var aggressorValue = CalculateMoveValue(aggressor, ItemCategory.Offense);
            var defenderValue = CalculateMoveValue(defender, ItemCategory.Defense);
            var damage = aggressorValue - defenderValue;
            results.Damage = damage;
            _logger.LogDebug($"{nameof(ExecuteAttackAsync)} Calculated damage:{damage}");

            // Attack caused damage
            if (damage > 0)
            {
                var result = defender.Health - damage;
                _logger.LogDebug($"{nameof(ExecuteAttackAsync)} Result:{result}");
                if (result <= 0)
                {
                    _logger.LogDebug($"{nameof(ExecuteAttackAsync)} Defender dies");
                    // Defender Died
                    await _dataService.UpdateCharacterHealthAsync(defender.Id, 0);
                    results.Plunder = await _dataService.UpdateCharacterNoInventoryAsync(defender.Id);

                    results.BattleIsOver = await EndBattle(battle.Id, aggressor.Id, defender.Id);
                }
                else
                {
                    await _dataService.UpdateCharacterHealthAsync(defender.Id, result);
                }
            }

            return results;
        }

        private async Task<BattleMoveResults> ExecuteSurrenderAsync(Battle battle, Character aggressor, Character defender)
        {
            _logger.LogDebug($"{nameof(ExecuteSurrenderAsync)} - {battle.Id}:{aggressor.Name}:{defender.Name}");

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
            _logger.LogDebug($"{nameof(ExecuteQuitAsync)} - {battle.Id}:{aggressor.Name}:{defender.Name}");

            var results = new BattleMoveResults();

            results.BattleIsOver = await EndBattle(battle.Id, aggressor.Id, defender.Id);

            return results;
        }

        private async Task<bool> EndBattle(int battleId, int aggressorId, int defenderId)
        {
            _logger.LogDebug($"{nameof(ExecuteQuitAsync)} - {battleId}:{aggressorId}:{defenderId}");

            await _dataService.UpdateBattleEndAsync(battleId);
            await _dataService.UpdateCharacterInBattleAsync(aggressorId, false);
            await _dataService.UpdateCharacterInBattleAsync(defenderId, false);
            return true;
        }

        private int CalculateMoveValue(Character character, ItemCategory category)
        {
            _logger.LogDebug($"{nameof(CalculateMoveValue)} - {character.Name}:{category}");

            var characterValue = GetCharacterValue(character, category);
            var primaryItemValue = GetItemValue(character.PrimaryItem, category);
            var secondaryItemValue = GetItemValue(character.SecondaryItem, category);
            var characterItemValue = characterValue + primaryItemValue + secondaryItemValue;

            _logger.LogDebug($"{nameof(CalculateMoveValue)} - CharacterItem value:{characterItemValue}");

            var dice = new Random();
            var diceValue = dice.Next(1, 6);

            _logger.LogDebug($"{nameof(CalculateMoveValue)} - Dice value:{diceValue}");

            var moveValue = characterItemValue * diceValue;

            _logger.LogDebug($"{nameof(CalculateMoveValue)} - Move value:{moveValue}");

            return moveValue;
        }

        private int GetCharacterValue(Character character, ItemCategory category)
        {
            _logger.LogDebug($"{nameof(GetCharacterValue)} - {character.Name}:{category}");

            var skill = character.Race?.Skill;
            var level = character.Level;

            var characterLevel = level;
            switch (category)
            {
                case ItemCategory.Offense:
                    switch (skill)
                    {
                        case CharacterSkill.Strength:
                            characterLevel = 10 + level;
                            break;
                        case CharacterSkill.Intelligence:
                            characterLevel = 5 + level;
                            break;
                    }
                    break;

                case ItemCategory.Defense:
                    switch (skill)
                    {
                        case CharacterSkill.Stealth:
                            characterLevel = 10 + level;
                            break;
                        case CharacterSkill.Intelligence:
                            characterLevel = 5 + level;
                            break;
                    }
                    break;
            }

            _logger.LogDebug($"{nameof(GetCharacterValue)} - Character value:{characterLevel}");

            return characterLevel;
        }

        private int GetItemValue(CharacterItem? item, ItemCategory category)
        {
            _logger.LogDebug($"{nameof(GetItemValue)} - {item}:{category}");

            var itemValueNullable = item?.Item?.Category == category ? item.Item?.Value : 0;
            var itemValue = itemValueNullable == null ? 0 : (int)itemValueNullable;

            _logger.LogDebug($"{nameof(GetItemValue)} - Item value:{itemValue}");

            return itemValue;
        }
    }
}
