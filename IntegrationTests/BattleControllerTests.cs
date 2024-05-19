using Games;
using Games.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Games.Models;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Runtime.Intrinsics.X86;
using Xunit.Abstractions;
using System.Net;

namespace IntegrationTests
{
    public class BattleControllerTests : IClassFixture<IntegrationTestsWebApplicationFactory<Program>>
    {
        private readonly ITestOutputHelper _output;

        private readonly IntegrationTestsWebApplicationFactory<Program> _factory;

        private readonly HttpClient _client;

        private string _password = "Test123!";

        private string _user1 = "user1@example.com";
        private string _token1 = "";

        private string _user2 = "user2@example.com";
        private string _token2 = "";

        public BattleControllerTests(IntegrationTestsWebApplicationFactory<Program> factory, ITestOutputHelper output)
        {
            _factory = factory;

            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            _output = output;
        }

        [Fact]
        public async Task Test_CharacterController_AttackToDeath()
        {
            await Login();

            var character1 = await CreateCharacter(_token1);
            var character2 = await CreateCharacter(_token2);

            // User1 Starts a Battle
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token1);
            var battleResponse = await _client.PostAsync($"/api/Battle?character1Id={character1.Id}&character2Id={character2.Id}", null);
            battleResponse.EnsureSuccessStatusCode();
            var battle = await battleResponse.Content.ReadFromJsonAsync<Battle>();
            Assert.NotNull(battle);
            Assert.True(battle.Id > 0);

            // User2 Accepts
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token2);
            var acceptResponse = await _client.PostAsync($"/api/Battle/{battle.Id}/Move?characterId={character2.Id}&move=Accept", null);
            acceptResponse.EnsureSuccessStatusCode();
            var acceptResults = await acceptResponse.Content.ReadFromJsonAsync<BattleMoveResults>();
            Assert.NotNull(acceptResults);

            HttpResponseMessage attackResponse;
            BattleMoveResults? attackResults;
            var attackNumber = 1;
            bool battleIsOver = false;
            while (!battleIsOver)
            {
                var token = _token1;
                var characterId = character1.Id;
                if (attackNumber %2 == 0)
                {
                    token = _token2;
                    characterId = character2.Id;
                }

                // Attack until death
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                attackResponse = await _client.PostAsync($"/api/Battle/{battle.Id}/Move?characterId={characterId}&move=Attack", null);
                attackResponse.EnsureSuccessStatusCode();
                attackResults = await attackResponse.Content.ReadFromJsonAsync<BattleMoveResults>();
                Assert.NotNull(attackResults);

                _output.WriteLine($"Attack {attackNumber}  Agressor {attackResults.BattleMove?.OpponentId}  Damage {attackResults.Damage}");

                battleIsOver = attackResults.BattleIsOver;
                attackNumber++;

                if (battleIsOver) _output.WriteLine($"Winner = {attackResults.BattleMove?.OpponentId}");
            }
            
        }

        private async Task Login()
        {
            var login1Response = await _client.PostAsJsonAsync("/login", 
                new { email = _user1, password = _password });
            var login1Status = login1Response.StatusCode;
            if (login1Status == HttpStatusCode.Unauthorized)
            {
                var register1Response = await _client.PostAsJsonAsync("/register",
                new { email = _user1, password = _password });
                register1Response.EnsureSuccessStatusCode();

                login1Response = await _client.PostAsJsonAsync("/login",
                    new { email = _user1, password = _password });
                login1Response.EnsureSuccessStatusCode();
            }
            var token1Response = await login1Response.Content.ReadFromJsonAsync<AccessTokenResponse>();
            Assert.NotNull(token1Response);
            _token1 = token1Response.AccessToken;

            var login2Response = await _client.PostAsJsonAsync("/login", 
                new { email = _user2, password = _password });
            var login2Status = login2Response.StatusCode;
            if (login2Status == HttpStatusCode.Unauthorized)
            {
                var register2Response = await _client.PostAsJsonAsync("/register",
                new { email = _user2, password = _password });
                register2Response.EnsureSuccessStatusCode();

                login2Response = await _client.PostAsJsonAsync("/login",
                    new { email = _user2, password = _password });
                login2Response.EnsureSuccessStatusCode();
            }
            var token2Response = await login2Response.Content.ReadFromJsonAsync<AccessTokenResponse>();
            Assert.NotNull(token2Response);
            _token2 = token2Response.AccessToken;
        }

        private async Task<Character> CreateCharacter(string token)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createResponse = await _client.PostAsync($"/api/Character?name=Test&raceId=1", null);
            createResponse.EnsureSuccessStatusCode();
            var characterResponse = await createResponse.Content.ReadFromJsonAsync<Character>();
            Assert.NotNull(characterResponse);
            var character = characterResponse;
            Assert.True(character.Id > 0);

            // Add a sword
            var swordResponse = await _client.PostAsync($"/api/Character/{character.Id}/Item?itemId=1", null);
            swordResponse.EnsureSuccessStatusCode();
            var sword = await swordResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(sword);
            Assert.True(sword.Id > 0);

            // Add a shield
            var shieldResponse = await _client.PostAsync($"/api/Character/{character.Id}/Item?itemId=3", null);
            shieldResponse.EnsureSuccessStatusCode();
            var shield = await shieldResponse.Content.ReadFromJsonAsync<CharacterItem>();
            Assert.NotNull(shield);
            Assert.True(shield.Id > 0);

            // Update primary item to sword
            var primaryResponse = await _client.PutAsync($"/api/Character/{character.Id}/PrimaryItem?inventoryItemId={sword.Id}", null);
            primaryResponse.EnsureSuccessStatusCode();

            // Update character secondary item to shield
            var secondaryResponse = await _client.PutAsync($"/api/Character/{character.Id}/SecondaryItem?inventoryItemId={shield.Id}", null);
            secondaryResponse.EnsureSuccessStatusCode();

            return character;
        }
    }
}