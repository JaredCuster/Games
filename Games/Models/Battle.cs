using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models
{
    public enum Move
    {
        Initiate = 1,
        Accept = 2,
        Attack = 3,
        Pursue = 4,
        Retreat = 5,
        Surrender = 6,
        Quit = 7
    }

    public class Battle
    {
        public int Id { get; set; }

        [Required]
        public int Opponent1Id { get; set; }

        [Required]
        public int Opponent2Id { get; set; }

        [Required]
        public bool Active { get; set; } = false;

        public int? LastMoveId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        [ForeignKey("Opponent1Id")]
        public Character? Opponent1 { get; set; }

        [ForeignKey("Opponent2Id")]
        public Character? Opponent2 { get; set; }

        [ForeignKey("LastMoveId")]
        public BattleMove? LastMove { get; set; }

        public ICollection<BattleMove> Moves { get; set; } = new List<BattleMove>();
    }

    public class BattleMove
    {
        public int Id { get; set; }

        [Required]
        public int BattleId { get; set; }

        [Required]
        public int OpponentId { get; set; }

        [Required]
        public Move Move { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }
    }

    public class BattleMoveResults
    {
        public bool BattleIsOver { get ; set; } = false;

        public BattleMove? BattleMove { get; set; }

        public int Damage { get; set; }

        public IEnumerable<int> Plunder { get; set; } = [];
    }
}
