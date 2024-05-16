using Games.Models;
using Microsoft.AspNetCore.Identity;

namespace Games.Services
{
    public interface IAdminService
    {
        public Task<ICollection<IdentityUser>> GetUsersAsync();

        public Task<IdentityUser> GetUserAsync(string email);

        public Task<ICollection<Character>> GetUserCharactersAsync(string email);

        public Task DeleteUserAsync(string email);
    }

    public class AdminService : IAdminService
    {
        private IAdminDataService _dataService;

        public AdminService(IAdminDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<ICollection<IdentityUser>> GetUsersAsync()
        {
            var users = await _dataService.GetUsersAsync();

            return users;
        }

        public async Task<IdentityUser> GetUserAsync(string email)
        {
            var users = await _dataService.GetUserAsync(email);

            return users;
        }

        public async Task<ICollection<Character>> GetUserCharactersAsync(string email)
        {
            var characters = await _dataService.GetUserCharactersAsync(email);

            return characters;
        }

        public async Task DeleteUserAsync(string email)
        {
            await _dataService.DeleteUserAsync(email);
        }
    }
}
