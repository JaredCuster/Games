using Games;
using Games.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Games.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Net.Http.Headers;
using Games.Services;
using System.Net;

namespace IntegrationTests
{
    public class CharacterControllerTests : IClassFixture<IntegrationTestsWebApplicationFactory<Program>>
    {
        private readonly IntegrationTestsWebApplicationFactory<Program> _factory;

        private readonly HttpClient _client;

        private bool _registered = false;
        private bool _loggedIn = false;

        public CharacterControllerTests(IntegrationTestsWebApplicationFactory<Program> factory)
        {
            _factory = factory;

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private async Task Register()
        {
            if (_registered) { return; }

            var registerResponse = await _client.PostAsJsonAsync("/register",
                new { email = "user1@example.com", password = "Test123!" });

            registerResponse.EnsureSuccessStatusCode();

            _registered = true;
        }

        private async Task Login()
        {
            if (_loggedIn) { return; }

            var loginResponse = await _client.PostAsJsonAsync("/login",
                new { email = "user1@example.com", password = "Test123!" });

            loginResponse.EnsureSuccessStatusCode();

            var token = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();
            Assert.NotNull(token);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(token.TokenType, token.AccessToken);

            _loggedIn = true;
        }

        [Fact]
        public async Task Test_CreateCharacter_Duplicate_ThrowsException()
        {
            await Register();
            await Login();

            var name = "Duplicate";
            var race = 1;

            var createResponse = await _client.PostAsync($"/api/Character?name={name}&raceId={race}", null);
            createResponse.EnsureSuccessStatusCode();
            var createCharacter = await createResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(createCharacter);
            Assert.True(createCharacter.Id > 0);

            var createDupResponse = await _client.PostAsync($"/api/Character?name={name}&raceId={race}", null);
            var dupStatus = createDupResponse.StatusCode;
            Assert.Equal(HttpStatusCode.BadRequest, dupStatus);
        }

        [Fact]
        public async Task Test_CharacterController()
        {
            await Register();
            await Login();

            // Create character
            var createResponse = await _client.PostAsync($"/api/Character?name=Test&raceId=1", null);
            createResponse.EnsureSuccessStatusCode();
            var createCharacter = await createResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(createCharacter);
            Assert.True(createCharacter.Id > 0);

            // Get characters
            var getsResponse = await _client.GetAsync("/api/Characters");
            getsResponse.EnsureSuccessStatusCode();
            var characters = await getsResponse.Content.ReadFromJsonAsync<IList<CharacterRace>>();
            Assert.NotNull(characters);
            Assert.True(characters.Count > 0);

            // Get character
            var getResponse = await _client.GetAsync($"/api/Character/{createCharacter.Id}");
            getResponse.EnsureSuccessStatusCode();
            var character = await getResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(character);

            var characterId = character.Id;

            // Update character name
            var updateResponse = await _client.PutAsync($"/api/Character/{characterId}/Name?name=NewTest", null);
            updateResponse.EnsureSuccessStatusCode();
            var updateCharacter = await updateResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(updateCharacter);
            Assert.Equal("NewTest", updateCharacter.Name);

            // Add a sword
            var swordResponse = await _client.PostAsync($"/api/Character/{characterId}/Item?itemId=1", null);
            swordResponse.EnsureSuccessStatusCode();
            var sword = await swordResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(sword);
            Assert.True(sword.Id > 0);

            // Add a shield
            var shieldResponse = await _client.PostAsync($"/api/Character/{characterId}/Item?itemId=3", null);
            shieldResponse.EnsureSuccessStatusCode();
            var shield = await shieldResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(shield);
            Assert.True(shield.Id > 0);

            // Add a armor
            var armorResponse = await _client.PostAsync($"/api/Character/{characterId}/Item?itemId=4", null);
            armorResponse.EnsureSuccessStatusCode();
            var armor = await armorResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(armor);
            Assert.True(armor.Id > 0);

            // Get character inventory
            var inventoryResponse = await _client.GetAsync($"/api/Character/{characterId}/Inventory");
            inventoryResponse.EnsureSuccessStatusCode();
            var inventory = await inventoryResponse.Content.ReadFromJsonAsync<IList<CharacterItem>>();
            Assert.NotNull(inventory);
            Assert.True(inventory.Count == 3);

            // Update character primary item
            var primaryResponse = await _client.PutAsync($"/api/Character/{characterId}/PrimaryItem?inventoryItemId={sword.Id}", null);
            primaryResponse.EnsureSuccessStatusCode();
            var primaryCharacter = await primaryResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(primaryCharacter);
            Assert.Equal(primaryCharacter.PrimaryItemId, sword.Id);

            // Update character secondary item
            var secondaryResponse = await _client.PutAsync($"/api/Character/{characterId}/SecondaryItem?inventoryItemId={shield.Id}", null);
            secondaryResponse.EnsureSuccessStatusCode();
            var secondaryCharacter = await secondaryResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(secondaryCharacter);
            Assert.Equal(secondaryCharacter.SecondaryItemId, shield.Id);

            // Get and Delete sword
            var getItemResponse = await _client.GetAsync($"/api/Character/Item/{sword.Id}");
            getItemResponse.EnsureSuccessStatusCode();
            sword = await getItemResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(sword);
            var deleteItemResponse = await _client.DeleteAsync($"/api/Character/Item/{sword.Id}");
            deleteItemResponse.EnsureSuccessStatusCode();

            // Get and Delete shield
            getItemResponse = await _client.GetAsync($"/api/Character/Item/{shield.Id}");
            getItemResponse.EnsureSuccessStatusCode();
            sword = await getItemResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(sword);
            deleteItemResponse = await _client.DeleteAsync($"/api/Character/Item/{shield.Id}");
            deleteItemResponse.EnsureSuccessStatusCode();

            // Get and Delete armor
            getItemResponse = await _client.GetAsync($"/api/Character/Item/{armor.Id}");
            getItemResponse.EnsureSuccessStatusCode();
            sword = await getItemResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(sword);
            deleteItemResponse = await _client.DeleteAsync($"/api/Character/Item/{armor.Id}");
            deleteItemResponse.EnsureSuccessStatusCode();

            // Check character inventory
            inventoryResponse = await _client.GetAsync($"/api/Character/{characterId}/Inventory");
            inventoryResponse.EnsureSuccessStatusCode();
            inventory = await inventoryResponse.Content.ReadFromJsonAsync<IList<CharacterItem>>();
            Assert.NotNull(inventory);
            Assert.True(inventory.Count == 0);

            // Check character
            getResponse = await _client.GetAsync($"/api/Character/{createCharacter.Id}");
            getResponse.EnsureSuccessStatusCode();
            character = await getResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(character);
            Assert.True(character.PrimaryItemId == null && character.SecondaryItemId == null);
        }
    }
}