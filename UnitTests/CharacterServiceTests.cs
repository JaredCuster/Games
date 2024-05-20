using Games.Models;
using Games.Services.ControllerServices;
using Games.Services.DataServices;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests
{
    public class CharacterServiceTests
    {
        public Mock<ILogger<ICharacterControllerService>> mockLogger = new Mock<ILogger<ICharacterControllerService>>();
        public Mock<ICharacterDataService> mockDataService = new Mock<ICharacterDataService>();

        [Fact]
        public async Task GetCharacters()
        {
            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.GetCharactersAsync();

            mockDataService.Verify(s => s.GetCharactersAsync(), Times.Once());
        }

        [Fact]
        public async Task GetDeceasedCharacters()
        {
            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.GetDeceasedCharactersAsync();

            mockDataService.Verify(s => s.GetDeceasedCharactersAsync(), Times.Once());
        }

        [Fact]
        public async Task GetCharacter()
        {
            const int id = 1;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.GetCharacterAsync(id);

            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public async Task AddCharacter()
        {
            const string name = "TestName";
            const int raceId = 1;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.AddCharacterAsync(name, raceId);

            mockDataService.Verify(s => s.AddCharacterAsync(
                It.Is<Character>(c =>
                    c.Name == name &&
                    c.RaceId == raceId &&
                    c.Health == 100 &&
                    c.Level == 1 &&
                    c.InBattle == false &&
                    c.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task UpdateCharacterName()
        {
            const int id = 1;
            const string name = "TestName";

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.UpdateCharacterNameAsync(id, name);

            mockDataService.Verify(s => s.UpdateCharacterNameAsync(
                It.Is<int>(i => i == id),
                It.Is<string>(s => s == name)), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public void UpdateCharacterName_Empty_ThrowsException()
        {
            const int id = 1;
            const string name = "";

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            var actual = () => characterService.UpdateCharacterNameAsync(id, name);

            Assert.ThrowsAsync<CharacterException>(actual);
        }

        [Fact]
        public async Task UpdateCharacterPrimaryItem_NullItem()
        {
            const int id = 1;
            const int itemId = 0;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.UpdateCharacterItemAsync(id, itemId, true);

            mockDataService.Verify(s => s.UpdateCharacterPrimaryItemAsync(
                It.Is<int>(i => i == id),
                null), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public async Task UpdateCharacterSecondaryItem_NullItem()
        {
            const int id = 1;
            const int itemId = 0;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.UpdateCharacterItemAsync(id, itemId, false);

            mockDataService.Verify(s => s.UpdateCharacterSecondaryItemAsync(
                It.Is<int>(i => i == id),
                null), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public async Task UpdateCharacterPrimaryItem()
        {
            const int id = 1;
            const int itemId = 1;
            var characterItem = CreateCharacterItem(id, itemId);
            var character = CreateCharacter(id);
            character.Inventory = [characterItem];

            mockDataService.Setup(ds => ds.GetCharacterAsync(It.IsAny<int>()))
                .ReturnsAsync(character);
            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.UpdateCharacterItemAsync(id, itemId, true);

            mockDataService.Verify(s => s.UpdateCharacterPrimaryItemAsync(
                It.Is<int>(i => i == id),
                It.Is<int>(i => i == itemId)), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateCharacterSecondaryItem()
        {
            const int id = 1;
            const int itemId = 1;
            var characterItem = CreateCharacterItem(id, itemId);
            var character = CreateCharacter(id);
            character.Inventory = [characterItem];

            mockDataService.Setup(ds => ds.GetCharacterAsync(It.IsAny<int>()))
                .ReturnsAsync(character);
            
            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.UpdateCharacterItemAsync(id, itemId, false);

            mockDataService.Verify(s => s.UpdateCharacterSecondaryItemAsync(
                It.Is<int>(i => i == id),
                It.Is<int>(i => i == itemId)), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Exactly(2));
        }

        [Fact]
        public void UpdateCharacterPrimaryItem_InUse_ThrowsException()
        {
            const int id = 1;
            const int itemId = 1;
            var characterItem = CreateCharacterItem(id, itemId);
            var character = CreateCharacter(id);
            character.PrimaryItemId = itemId;
            character.Inventory = [characterItem];

            mockDataService.Setup(ds => ds.GetCharacterAsync(It.IsAny<int>()))
                .ReturnsAsync(character);

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            var actual = () => characterService.UpdateCharacterItemAsync(id, itemId, true);

            Assert.ThrowsAsync<CharacterException>(actual);
        }

        [Fact]
        public void UpdateCharacterSecondaryItem_InUse_ThrowsException()
        {
            const int id = 1;
            const int itemId = 1;
            var characterItem = CreateCharacterItem(id, itemId);
            var character = CreateCharacter(id);
            character.SecondaryItemId = itemId;
            character.Inventory = [characterItem];

            mockDataService.Setup(ds => ds.GetCharacterAsync(It.IsAny<int>()))
                .ReturnsAsync(character);

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            var actual = () => characterService.UpdateCharacterItemAsync(id, itemId, false);

            Assert.ThrowsAsync<CharacterException>(actual);
        }

        [Fact]
        public async Task DeleteCharacter()
        {
            mockDataService.Setup(ds => ds.DeleteCharacterAsync(It.IsAny<int>()));

            const int id = 1;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.DeleteCharacterAsync(id);

            mockDataService.Verify(s => s.DeleteCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public async Task GetCharacterItems()
        {
            const int id = 1;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.GetCharacterItemsAsync(id);

            mockDataService.Verify(s => s.GetCharacterItemsAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public async Task GetCharacterItem()
        {
            const int id = 1;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.GetCharacterItemAsync(id);

            mockDataService.Verify(s => s.GetCharacterItemAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public async Task AddCharacterItem()
        {
            const int id = 1;
            const int itemId = 1;
            
            mockDataService.Setup(ds => ds.GetCharacterItemsAsync(It.IsAny<int>()))
                .ReturnsAsync([]);

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.AddCharacterItemAsync(id, itemId);

            mockDataService.Verify(s => s.AddCharacterItemAsync(
                It.Is<int>(i => i == id),
                It.Is<CharacterItem>(ci =>
                    ci.CharacterId == id &&
                    ci.ItemId == itemId
                )), Times.Once());
            mockDataService.Verify(s => s.GetCharacterItemAsync(
                It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public void AddCharacterItem_InsufficientCapacity_ThrowsException()
        {
            const int id = 1;
            const int itemId = 1;
            var items = new List<CharacterItem>() {
                CreateCharacterItem(id, itemId),
                CreateCharacterItem(id, itemId),
                CreateCharacterItem(id, itemId)
            };

            mockDataService.Setup(ds => ds.GetCharacterItemsAsync(It.IsAny<int>()))
                .ReturnsAsync(items);

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            var actual = () => characterService.AddCharacterItemAsync(id, itemId);

            Assert.ThrowsAsync<CharacterException>(actual);
        }

        [Fact]
        public async Task DeleteCharacterItem_NoCharacterItem()
        {
            const int id = 1;
            const int characterItemId = 1;
            var characterItem = CreateCharacterItem(id, characterItemId);
            var character = CreateCharacter(id);
            character.Inventory = [characterItem];

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetCharacterItemAsync(It.IsAny<int>()))
                .ReturnsAsync(characterItem);
            mockDataService.Setup(ds => ds.GetCharacterAsync(It.IsAny<int>()))
                .ReturnsAsync(character);

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.DeleteCharacterItemAsync(characterItemId);

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetCharacterItemAsync(
                It.Is<int>(i => i == characterItemId)), Times.Once());
            mockDataService.Verify(s => s.DeleteCharacterItemAsync(
                It.Is<int>(i => i == characterItemId)), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());
        }

        [Fact]
        public async Task DeleteCharacterItem_PrimaryCharacterItem()
        {
            const int id = 1;
            const int characterItemId = 1;
            var characterItem = CreateCharacterItem(id, characterItemId);
            var character = CreateCharacter(id);
            character.Inventory = [characterItem];
            character.PrimaryItemId = characterItemId;

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetCharacterItemAsync(It.IsAny<int>()))
                .ReturnsAsync(characterItem);
            mockDataService.Setup(ds => ds.GetCharacterAsync(It.IsAny<int>()))
                .ReturnsAsync(character);

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.DeleteCharacterItemAsync(characterItemId);

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.DeleteCharacterItemAsync(
                It.Is<int>(i => i == characterItemId)), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterPrimaryItemAsync(
                It.Is<int>(i => i == id),
                null), Times.Once());
            mockDataService.Verify(s => s.GetCharacterItemAsync(
                It.Is<int>(i => i == characterItemId)), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());
        }

        [Fact]
        public async Task DeleteCharacterItem_SecondaryCharacterItem()
        {
            const int id = 1;
            const int characterItemId = 1;
            var characterItem = CreateCharacterItem(id, characterItemId);
            var character = CreateCharacter(id);
            character.Inventory = [characterItem];
            character.SecondaryItemId = characterItemId;

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetCharacterItemAsync(It.IsAny<int>()))
                .ReturnsAsync(characterItem);
            mockDataService.Setup(ds => ds.GetCharacterAsync(It.IsAny<int>()))
                .ReturnsAsync(character);

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.DeleteCharacterItemAsync(characterItemId);

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetCharacterItemAsync(
                It.Is<int>(i => i == characterItemId)), Times.Once());
            mockDataService.Verify(s => s.DeleteCharacterItemAsync(
                It.Is<int>(i => i == characterItemId)), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == id)), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterSecondaryItemAsync(
                It.Is<int>(i => i == id),
                null), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());
        }

        [Fact]
        public async Task GetCharacterBattles()
        {
            const int id = 1;

            var characterService = new CharacterControllerService(mockLogger.Object, mockDataService.Object);
            await characterService.GetCharacterBattlesAsync(id);

            mockDataService.Verify(s => s.GetCharacterBattlesAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        private Character CreateCharacter(int? id = null)
        {
            var character = new Character()
            {
                Name = "TestUser",
                RaceId = 1,
                Health = 100,
                Level = 1,
                InBattle = false,
                CreateDate = DateTime.Now
            }; 

            if (id != null)
            {
                character.Id = (int)id;
            }
            
            return character;
        }

        private CharacterItem CreateCharacterItem(int characterId, int itemId)
        {
            var characterItem = new CharacterItem()
            {
                Id = 1,
                CharacterId = characterId,
                ItemId = itemId
            };

            return characterItem;
        }
    }
}