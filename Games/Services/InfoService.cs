using Games.Data;
using Games.Models;
using Microsoft.EntityFrameworkCore;

namespace Games.Services
{
    public interface IInfoService
    {
        public Task<IEnumerable<CharacterRace>> GetRacesAsync();

        public Task<IEnumerable<Item>> GetItemsAsync();
    }

    public class InfoService : IInfoService
    {
        private IInfoDataService _dataService;

        public InfoService(IInfoDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IEnumerable<CharacterRace>> GetRacesAsync()
        {
            var races = await _dataService.GetRacesAsync();

            return races;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            var items = await _dataService.GetItemsAsync();

            return items;
        }
    }
}
