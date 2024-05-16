using Games;
using Games.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Games.Models;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class InfoControllerTests : IClassFixture<IntegrationTestsWebApplicationFactory<Program>>
    {
        private readonly IntegrationTestsWebApplicationFactory<Program> _factory;

        private readonly HttpClient _client;

        public InfoControllerTests(IntegrationTestsWebApplicationFactory<Program> factory)
        {
            _factory = factory;

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task Test_InfoController()
        {
            var racesResponse = await _client.GetAsync("/api/Info/Races");
            racesResponse.EnsureSuccessStatusCode();
            var races = await racesResponse.Content.ReadFromJsonAsync<IList<CharacterRace>>();
            Assert.NotNull(races);

            var itemsResponse = await _client.GetAsync("/api/Info/Items");
            itemsResponse.EnsureSuccessStatusCode();
            var items = await itemsResponse.Content.ReadFromJsonAsync<IList<Item>>();
            Assert.NotNull(items);
        }
    }
}