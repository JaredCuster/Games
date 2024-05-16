using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Claims;

namespace Games.Services
{
    public interface IDataService
    {
        public IDbContextTransaction BeginTransaction();

        public void CommitTransaction(IDbContextTransaction transaction);
    }

    public abstract class BaseDataService
    {
        protected DataContext _dataContext;

        protected IHttpContextAccessor _httpContextAccessor;

        public BaseDataService(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetLoggedInUserId()
        {
            var ownerId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(ownerId)) { throw new NotFoundException("Owner not found"); }

            return ownerId;
        }

        protected async Task<Character> GetCharacterForUpdateAsync(int id)
        {
            var character = await _dataContext.Characters.FindAsync(id);
            if (character == null) { throw new NotFoundException($"Character - ({id}) not found"); }

            return character;
        }

        protected async Task<Battle> GetBattleForUpdateAsync(int id)
        {
            var battle = await _dataContext.Battles.FindAsync(id);
            if (battle == null) { throw new NotFoundException($"Battle - ({id}) not found"); }

            return battle;
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _dataContext.Database.BeginTransaction();
        }

        public void CommitTransaction(IDbContextTransaction transaction)
        {
            transaction.Commit();
        }
    }
}
