using Games.Data;
using Games.Models;
using Games.Services.DataServices;

namespace Games.Services.ControllerServices
{
    public interface IInfoControllerService
    {
        public Task<IEnumerable<CharacterRace>> GetRacesAsync();

        public Task<IEnumerable<Item>> GetItemsAsync();
    }

    public class InfoControllerService : IInfoControllerService
    {
        private readonly ILogger<IInfoControllerService> _logger;
        private readonly IInfoDataService _dataService;

        public InfoControllerService(ILogger<IInfoControllerService> logger, IInfoDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<IEnumerable<CharacterRace>> GetRacesAsync()
        {
            _logger.LogDebug($"{nameof(GetRacesAsync)}");

            var races = await _dataService.GetRacesAsync();

            return races;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            _logger.LogDebug($"{nameof(GetRacesAsync)}");

            var items = await _dataService.GetItemsAsync();

            return items;
        }
    }
}
