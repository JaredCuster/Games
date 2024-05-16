using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Games.Models
{
    public enum CharacterSkill
    {
        None = 1,
        Strength = 2,
        Intelligence = 3,
        Stealth = 4
    }

    public class CharacterRace
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public CharacterSkill Skill { get; set; }
    }

    public class Character
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public int RaceId { get; set; }

        [Required]
        [Range(0, 100)]
        public int Health { get; set; } = 100;

        [Required]
        [Range(1, 10)]
        public int Level { get; set; } = 1;

        public int? PrimaryItemId { get; set; }

        public int? SecondaryItemId { get; set; }

        [Required]
        public bool InBattle { get; set; } = false;

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public string OwnerId { get; set; } = null!;

        [ForeignKey("RaceId")]
        public CharacterRace? Race { get; set; }

        [ForeignKey("PrimaryItemId")]
        public CharacterItem? PrimaryItem { get; set; }

        [ForeignKey("SecondaryItemId")]
        public CharacterItem? SecondaryItem { get; set; }

        public ICollection<CharacterItem> Inventory { get; set; } = new List<CharacterItem>();
    }

    public class CharacterItem
    {
        public int Id { get; set; }

        public int CharacterId { get; set; }

        public int ItemId { get; set; }

        [ForeignKey("ItemId")]
        public Item? Item { get; set; }
    }
}
