using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Claims;

namespace Games.Services.DataServices
{
    public interface IDataService
    {
        public IDbContextTransaction BeginTransaction();

        public void CommitTransaction(IDbContextTransaction transaction);
    }

    public abstract class BaseDataService
    {
        protected readonly ILogger _logger;

        protected readonly DataContext _dataContext;

        protected readonly IHttpContextAccessor _httpContextAccessor;

        public BaseDataService(ILogger logger, DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetLoggedInUserId()
        {
            _logger.LogDebug($"{nameof(GetLoggedInUserId)}");

            var ownerId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId)) 
            {
                var msg = $"Owner not found";
                _logger.LogWarning($"{nameof(GetLoggedInUserId)} {msg}");
                throw new NotFoundException(msg); 
            }

            return ownerId;
        }

        protected async Task<Character> GetCharacterForUpdateAsync(int id)
        {
            _logger.LogDebug($"{nameof(GetCharacterForUpdateAsync)}");

            var character = await _dataContext.Characters.FindAsync(id);
            if (character == null) 
            {
                var msg = $"Character ({id}) not found";
                _logger.LogWarning($"{nameof(GetCharacterForUpdateAsync)} {msg}");
                throw new NotFoundException(msg); 
            }

            return character;
        }

        protected async Task<Battle> GetBattleForUpdateAsync(int id)
        {
            _logger.LogDebug($"{nameof(GetBattleForUpdateAsync)}");

            var battle = await _dataContext.Battles.FindAsync(id);
            if (battle == null) 
            {
                var msg = $"Battle ({id}) not found";
                _logger.LogWarning($"{nameof(GetBattleForUpdateAsync)} {msg}");
                throw new NotFoundException(msg);
            }

            return battle;
        }

        public IDbContextTransaction BeginTransaction()
        {
            _logger.LogDebug($"{nameof(BeginTransaction)}");

            return _dataContext.Database.BeginTransaction();
        }

        public void CommitTransaction(IDbContextTransaction transaction)
        {
            _logger.LogDebug($"{nameof(CommitTransaction)}");

            transaction.Commit();
        }
    }
}
