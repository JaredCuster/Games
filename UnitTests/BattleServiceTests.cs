using Games.Models;
using Games.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using System.Xml.Linq;

namespace UnitTests
{
    public class BattleServiceTests
    {
        public Mock<IBattleDataService> mockDataService = new Mock<IBattleDataService>();

        [Fact]
        public async Task GetBattles()
        {
            var battleService = new BattleService(mockDataService.Object);
            await battleService.GetBattlesAsync();

            mockDataService.Verify(s => s.GetBattlesAsync(), Times.Once());
        }

        [Fact]
        public async Task GetBattle()
        {
            const int id = 1;

            var battleService = new BattleService(mockDataService.Object);
            await battleService.GetBattleAsync(id);

            mockDataService.Verify(s => s.GetBattleAsync(
                It.Is<int>(i => i == id)), Times.Once());
        }

        [Fact]
        public async Task AddBattle()
        {
            int opponent1Id = 1;
            var character1 = CreateCharacter(opponent1Id);
            int opponent2Id = 2;
            var character2 = CreateCharacter(opponent2Id);

            int battleId = 1;
            int battleMoveId = 1;

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);
            mockDataService.Setup(ds => ds.AddBattleAsync(It.IsAny<Battle>()))
                .ReturnsAsync(battleId);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(It.IsAny<Battle>());

            var battleService = new BattleService(mockDataService.Object);
            await battleService.AddBattleAsync(opponent1Id, opponent2Id);

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == opponent1Id)), Times.Once());
            mockDataService.Verify(s => s.GetCharacterAsync(
                It.Is<int>(i => i == opponent2Id)), Times.Once());
            mockDataService.Verify(s => s.AddBattleAsync(
                It.Is<Battle>(b =>
                    b.Opponent1Id == opponent1Id &&
                    b.Opponent2Id == opponent2Id &&
                    b.Active == true &&
                    b.StartDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == opponent1Id &&
                    bm.Move == Move.Initiate &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.UpdateBattleLastMoveAsync(
                It.Is<int>(i => i == battleId),
                It.Is<int>(i => i == battleMoveId)), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterInBattleAsync(
                It.Is<int>(i => i == opponent1Id),
                true), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterInBattleAsync(
                It.Is<int>(i => i == opponent2Id),
                true), Times.Once());
            mockDataService.Verify(s => s.GetBattleAsync(
                It.Is<int>(i => i == battleId)), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());
        }

        [Fact]
        public void AddBattle_Opponent1_InBattle_ThrowsException()
        {
            int opponent1Id = 1;
            var character1 = CreateCharacter(opponent1Id);
            character1.InBattle = true;

            int opponent2Id = 2;
            var character2 = CreateCharacter(opponent2Id);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var actual = () => battleService.AddBattleAsync(opponent1Id, opponent2Id);

            Assert.ThrowsAsync<BattleException>(actual);
        }

        [Fact]
        public void AddBattle_Opponent2_InBattle_ThrowsException()
        {
            int opponent1Id = 1;
            var character1 = CreateCharacter(opponent1Id);

            int opponent2Id = 2;
            var character2 = CreateCharacter(opponent2Id);
            character1.InBattle = true;

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var actual = () => battleService.AddBattleAsync(opponent1Id, opponent2Id);

            Assert.ThrowsAsync<BattleException>(actual);
        }
        [Fact]
        public void AddBattleMove_InvalidMove_ThrowsException()
        {
            var battleService = new BattleService(mockDataService.Object);
            var actual = () => battleService.AddBattleMoveAsync(1, 2, "InvalidMove");

            Assert.ThrowsAsync<BattleException>(actual);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveInitiate_Accept()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);

            var aggressorId = opponent2Id;
            var defenderId = opponent1Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Initiate);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Accept.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Accept &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveInitiate_Retreat()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);

            var aggressorId = opponent2Id;
            var defenderId = opponent1Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Initiate);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Retreat.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Retreat &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveAccept_Attack()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);

            var aggressorId = opponent1Id;
            var defenderId = opponent2Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Accept);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Attack.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Attack &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterHealthAsync(
                It.Is<int>(i => i == defenderId), 
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveAccept_Attack_DefenderDies()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);
            character2.Health = 1;

            var aggressorId = opponent1Id;
            var defenderId = opponent2Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Accept);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Attack.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Attack &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterHealthAsync(
                It.Is<int>(i => i == defenderId),
                It.Is<int>(i => i == 0)), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterNoInventoryAsync(
               It.Is<int>(i => i == defenderId)), Times.Once());
            mockDataService.Verify(s => s.UpdateBattleEndAsync(
               It.Is<int>(i => i == battleId)), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterInBattleAsync(
                aggressorId, false), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterInBattleAsync(
                defenderId, false), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveAccept_Surrender()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);
            var initialHealth = character2.Health;
            var surrenderedHealth = initialHealth / 2;

            var aggressorId = opponent1Id;
            var defenderId = opponent2Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Accept);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Surrender.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Surrender &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterHealthAsync(
                It.Is<int>(i => i == defenderId),
                surrenderedHealth), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterNoInventoryAsync(
               It.Is<int>(i => i == defenderId)), Times.Once());
            mockDataService.Verify(s => s.UpdateBattleEndAsync(
               It.Is<int>(i => i == battleId)), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveAttack_Attack()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);

            var aggressorId = opponent2Id;
            var defenderId = opponent1Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Attack);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Attack.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Attack &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterHealthAsync(
                It.Is<int>(i => i == defenderId),
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveAttack_Attack_DefenderDies()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);
            
            character1.Health = 1;

            var aggressorId = opponent2Id;
            var defenderId = opponent1Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Attack);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Attack.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Attack &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterHealthAsync(
                It.Is<int>(i => i == defenderId),
                It.Is<int>(i => i == 0)), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterNoInventoryAsync(
               It.Is<int>(i => i == defenderId)), Times.Once());
            mockDataService.Verify(s => s.UpdateBattleEndAsync(
               It.Is<int>(i => i == battleId)), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterInBattleAsync(
                aggressorId, false), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterInBattleAsync(
                defenderId, false), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveAttack_Retreat()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);

            var aggressorId = opponent2Id;
            var defenderId = opponent1Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Attack);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Retreat.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Retreat &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveAttack_Surrender()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);
            var initialHealth = character2.Health;
            var surrenderedHealth = initialHealth / 2;

            var aggressorId = opponent2Id;
            var defenderId = opponent1Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Attack);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Surrender.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Surrender &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterHealthAsync(
                It.Is<int>(i => i == defenderId),
                surrenderedHealth), Times.Once());
            mockDataService.Verify(s => s.UpdateCharacterNoInventoryAsync(
               It.Is<int>(i => i == defenderId)), Times.Once());
            mockDataService.Verify(s => s.UpdateBattleEndAsync(
               It.Is<int>(i => i == battleId)), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        [Fact]
        public async Task AddBattleMove_LastMoveRetreat_Pursue()
        {
            int opponent1Id = 1;
            int opponent2Id = 2;
            var character1 = CreateCharacter(opponent1Id);
            var character2 = CreateCharacter(opponent2Id);
            var initialHealth = character2.Health;
            var surrenderedHealth = initialHealth / 2;

            var aggressorId = opponent1Id;
            var defenderId = opponent2Id;

            int battleId = 1;
            int battleMoveId = 1;
            var lastMove = CreateBattleMove(battleMoveId, battleId, defenderId, Move.Retreat);
            var battle = CreateBattle(battleId, opponent1Id, opponent2Id, lastMove);

            var transaction = new Mock<IDbContextTransaction>();
            transaction.Setup(t => t.TransactionId);
            var transactionObj = transaction.Object;

            mockDataService.Setup(ds => ds.BeginTransaction())
                .Returns(transactionObj);
            mockDataService.Setup(ds => ds.GetBattleAsync(It.IsAny<int>()))
                .ReturnsAsync(battle);
            mockDataService.Setup(ds => ds.AddBattleMoveAsync(It.IsAny<BattleMove>()))
                .ReturnsAsync(battleMoveId);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent1Id))
                .ReturnsAsync(character1);
            mockDataService.Setup(ds => ds.GetCharacterAsync(opponent2Id))
               .ReturnsAsync(character2);

            var battleService = new BattleService(mockDataService.Object);
            var results = await battleService.AddBattleMoveAsync(battleId, aggressorId, Move.Pursue.ToString());

            mockDataService.Verify(s => s.BeginTransaction(), Times.Once);
            mockDataService.Verify(s => s.GetBattleAsync(
                It.IsAny<int>()), Times.Once());
            mockDataService.Verify(s => s.AddBattleMoveAsync(
                It.Is<BattleMove>(bm =>
                    bm.BattleId == battleId &&
                    bm.OpponentId == aggressorId &&
                    bm.Move == Move.Pursue &&
                    bm.CreateDate.Date == DateTime.Now.Date
                )), Times.Once());
            mockDataService.Verify(s => s.CommitTransaction(
                It.Is<IDbContextTransaction>(t => t == transactionObj)), Times.Once());

            Assert.NotNull(results);
        }

        private Character CreateCharacter(int id)
        {
            var primaryItem = new CharacterItem()
            {
                Id = 1,
                CharacterId = id,
                ItemId = 1,
                Item = new() { Id = 1, Name = "Sword", Value = 10, Capacity = 0, Category = ItemCategory.Offense }
            };

            var character = new Character()
            {
                Id = id,
                Name = "TestUser",
                RaceId = 1,
                Health = 100,
                Level = 1,
                InBattle = false,
                CreateDate = DateTime.Now,
                PrimaryItemId = primaryItem.Id,
                PrimaryItem = primaryItem
            };

            return character;
        }

        private Battle CreateBattle(int id, int opponent1Id, int opponent2Id, BattleMove lastMove)
        {
            var battle = new Battle()
            {
                Id = id,
                Opponent1Id = opponent1Id,
                Opponent2Id = opponent2Id,
                Active = true,
                LastMoveId = lastMove.Id,
                StartDate = DateTime.Now.AddDays(-1),
                LastMove = lastMove
            };

            return battle;
        }

        private BattleMove CreateBattleMove(int id, int battleId, int opponentId, Move move)
        {
            var battleMove = new BattleMove()
            {
                Id = id,
                BattleId = battleId,
                OpponentId = opponentId,
                Move = move,
                CreateDate = DateTime.Now.AddHours(-1)
            };

            return battleMove;
        }
    }
}