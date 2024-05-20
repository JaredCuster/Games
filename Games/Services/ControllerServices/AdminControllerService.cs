using Games.Models;
using Games.Services.DataServices;
using Microsoft.AspNetCore.Identity;

namespace Games.Services.ControllerServices
{
    public interface IAdminControllerService
    {
        public Task<ICollection<IdentityUser>> GetUsersAsync();

        public Task<IdentityUser> GetUserAsync(string email);

        public Task<ICollection<Character>> GetUserCharactersAsync(string email);

        public Task DeleteUserAsync(string email);
    }

    public class AdminControllerService : IAdminControllerService
    {
        private readonly ILogger<IAdminControllerService> _logger;
        private readonly IAdminDataService _dataService;

        public AdminControllerService(ILogger<IAdminControllerService> logger, IAdminDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<ICollection<IdentityUser>> GetUsersAsync()
        {
            _logger.LogDebug($"{nameof(GetUsersAsync)}");
            
            var users = await _dataService.GetUsersAsync();
            
            return users;
        }

        public async Task<IdentityUser> GetUserAsync(string email)
        {
            _logger.LogDebug($"{nameof(GetUserAsync)} - {email}");

            var users = await _dataService.GetUserAsync(email);

            return users;
        }

        public async Task<ICollection<Character>> GetUserCharactersAsync(string email)
        {
            _logger.LogDebug($"{nameof(GetUserCharactersAsync)} - {email}");

            var characters = await _dataService.GetUserCharactersAsync(email);

            return characters;
        }

        public async Task DeleteUserAsync(string email)
        {
            _logger.LogDebug($"{nameof(DeleteUserAsync)} - {email}");

            await _dataService.DeleteUserAsync(email);
        }
    }
}
