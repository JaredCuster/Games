using System.ComponentModel.DataAnnotations;

namespace Games.Models
{
    public enum ItemCategory
    {
        None = 0, 
        Offense = 1,
        Defense = 2,
        Healing = 3
    }

    public class Item
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 100)]
        public int Value { get; set; } = 0;

        [Required]
        [Range(0, 100)]
        public int Capacity { get; set; } = 0;

        [Required]
        public ItemCategory Category { get; set; }
    }
}
