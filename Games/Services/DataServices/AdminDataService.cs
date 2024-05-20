using Games.Data;
using Games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Games.Services.DataServices
{
    public interface IAdminDataService : IDataService
    {
        public Task<ICollection<IdentityUser>> GetUsersAsync();

        public Task<IdentityUser> GetUserAsync(string email);

        public Task<ICollection<Character>> GetUserCharactersAsync(string email);

        public Task DeleteUserAsync(string email);
    }

    public class AdminDataService : BaseDataService, IAdminDataService
    {
        public AdminDataService(ILogger<IAdminDataService> logger, DataContext dataContext, IHttpContextAccessor httpContextAccessor) :
            base(logger, dataContext, httpContextAccessor)
        {

        }

        public async Task<ICollection<IdentityUser>> GetUsersAsync()
        {
            _logger.LogDebug($"{nameof(GetUsersAsync)}");

            var users = await _dataContext.Users.ToListAsync();

            return users;
        }

        public async Task<IdentityUser> GetUserAsync(string email)
        {
            _logger.LogDebug($"{nameof(GetUserAsync)} - {email}");

            var user = await _dataContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) 
            {
                var msg = $"User ({email}) not found";
                _logger.LogWarning($"{nameof(GetUserAsync)} {msg}");
                throw new NotFoundException(msg);
            }

            return user;
        }

        public async Task<ICollection<Character>> GetUserCharactersAsync(string email)
        {
            _logger.LogDebug($"{nameof(GetUserCharactersAsync)} - {email}");

            var owner = await GetUserAsync(email);

            var characters = await _dataContext.Characters
               .Where(c => c.OwnerId == email)
               .ToListAsync();

            return characters;
        }

        public async Task DeleteUserAsync(string email)
        {
            _logger.LogDebug($"{nameof(DeleteUserAsync)} - {email}");

            var user = await _dataContext.Users.FindAsync(email);
            if (user != null)
            {
                _dataContext.Remove(user);
                await _dataContext.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation($"{nameof(GetUserAsync)} User ({email}) not found");
            }
        }
    }
}
