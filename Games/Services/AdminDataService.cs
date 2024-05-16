using Games.Data;
using Games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Games.Services
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
        public AdminDataService(DataContext dataContext, IHttpContextAccessor httpContextAccessor) : 
            base(dataContext, httpContextAccessor)
        {
            
        }

        public async Task<ICollection<IdentityUser>> GetUsersAsync()
        {
            var users = await _dataContext.Users.ToListAsync();

            return users;
        }

        public async Task<IdentityUser> GetUserAsync(string email)
        {
            var user = await _dataContext.Users.Where(u => u.Email == email).FirstOrDefaultAsync();
            if (user == null) { throw new NotFoundException($"User - ({email}) not found"); }

            return (user);
        }

        public async Task<ICollection<Character>> GetUserCharactersAsync(string email)
        {
            var owner = await GetUserAsync(email);

            var characters = await _dataContext.Characters
               .Where(c => c.OwnerId == email)
               .ToListAsync();

            return characters;
        }

        public async Task DeleteUserAsync(string email)
        {
            var user = await _dataContext.Users.FindAsync(email);
            if (user != null)
            {
                _dataContext.Remove(user);
                await _dataContext.SaveChangesAsync();
            }
        }
    }
}
