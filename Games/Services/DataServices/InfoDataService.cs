using Games.Data;
using Games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Games.Services.DataServices
{
    public interface IInfoDataService : IDataService
    {
        public Task<IEnumerable<CharacterRace>> GetRacesAsync();

        public Task<IEnumerable<Item>> GetItemsAsync();
    }

    public class InfoDataService : BaseDataService, IInfoDataService
    {
        public InfoDataService(ILogger<IInfoDataService> logger, DataContext dataContext, IHttpContextAccessor httpContextAccessor) :
            base(logger, dataContext, httpContextAccessor)
        {

        }

        public async Task<IEnumerable<CharacterRace>> GetRacesAsync()
        {
            _logger.LogDebug($"{nameof(GetRacesAsync)}");

            var races = await _dataContext.CharacterRaces.ToListAsync();

            return races;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            _logger.LogDebug($"{nameof(GetItemsAsync)}");

            var items = await _dataContext.Items.ToListAsync();

            return items;
        }
    }
}
