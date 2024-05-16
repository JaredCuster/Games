using Games.Data;
using Games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Games.Services
{
    public interface IInfoDataService : IDataService
    {
        public Task<IEnumerable<CharacterRace>> GetRacesAsync();

        public Task<IEnumerable<Item>> GetItemsAsync();
    }

    public class InfoDataService : BaseDataService, IInfoDataService
    {
        public InfoDataService(DataContext dataContext, IHttpContextAccessor httpContextAccessor) : 
            base(dataContext, httpContextAccessor)
        {
            
        }

        public async Task<IEnumerable<CharacterRace>> GetRacesAsync()
        {
            var races = await _dataContext.CharacterRaces.ToListAsync();

            return races;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync()
        {
            var items = await _dataContext.Items.ToListAsync();

            return items;
        }
    }
}
