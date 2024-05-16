using Games.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Games.Data
{
    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base (options)
        {
            Database.EnsureCreated();
        }

        public virtual DbSet<Character> Characters { get; set; } = null!;

        public virtual DbSet<CharacterRace> CharacterRaces { get; set; } = null!;

        public virtual DbSet<CharacterItem> CharacterItems { get; set; } = null!;

        public virtual DbSet<Item> Items { get; set; } = null!;

        public virtual DbSet<Battle> Battles { get; set; } = null!;

        public virtual DbSet<BattleMove> BattleMoves { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed data
            var adminRole = new IdentityRole("Admin")
            {
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };
            modelBuilder.Entity<IdentityRole>().HasData(adminRole);

            var adminUser = new IdentityUser("admin") 
            {
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
            };
            var passwordHasher = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Test123!");
            modelBuilder.Entity<IdentityUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRole.Id,
                UserId = adminUser.Id
            });

            modelBuilder.Entity<CharacterRace>().HasData(new List<CharacterRace>
            {
                new() { Id = 1, Name = "Human", Skill = CharacterSkill.Intelligence },
                new() { Id = 2, Name = "Elf", Skill = CharacterSkill.Stealth },
                new() { Id = 3, Name = "Hobbit", Skill = CharacterSkill.None },
                new() { Id = 4, Name = "Dwarf", Skill = CharacterSkill.Strength },
                new() { Id = 5, Name = "Orc", Skill = CharacterSkill.Strength },
                new() { Id = 6, Name = "Ent", Skill = CharacterSkill.None },
                new() { Id = 7, Name = "Troll", Skill = CharacterSkill.None }
            });

            modelBuilder.Entity<Item>().HasData(new List<Item>
            {
                new() { Id=1, Name = "Sword", Value = 10, Capacity = 0, Category = ItemCategory.Offense },
                new() { Id=2, Name = "Bow", Value = 5, Capacity = 0, Category = ItemCategory.Offense },
                new() { Id=3, Name = "Shield", Value = 10, Capacity = 0, Category = ItemCategory.Defense },
                new() { Id=4, Name = "Armor", Value = 10, Capacity = 3, Category = ItemCategory.Defense },
                new() { Id=5, Name = "Food", Value = 10, Capacity = 0, Category = ItemCategory.Healing },
                new() { Id=6, Name = "First Aid", Value = 25, Capacity = 0, Category = ItemCategory.Healing },
                new() { Id=7, Name = "Small Case", Value = 0, Capacity = 5, Category = ItemCategory.None },
                new() { Id=8, Name = "Medium Case", Value = 0, Capacity = 10, Category = ItemCategory.None },
                new() { Id=9, Name = "Large Case", Value = 0, Capacity = 20, Category = ItemCategory.None }
            });

            // Mappings
            modelBuilder.Entity<Character>()
                .HasMany(c => c.Inventory)
                .WithOne()
                .HasForeignKey(c => c.CharacterId);

            modelBuilder.Entity<Battle>()
                .HasMany(b => b.Moves)
                .WithOne()
                .HasForeignKey(m => m.BattleId);

            // Navigations
            modelBuilder.Entity<CharacterItem>()
               .Navigation(ci => ci.Item)
               .AutoInclude();

            modelBuilder.Entity<Character>()
                .Navigation(c => c.Race)
                .AutoInclude();
            modelBuilder.Entity<Character>()
                .Navigation(c => c.PrimaryItem)
                .AutoInclude();
            modelBuilder.Entity<Character>()
                .Navigation(c => c.SecondaryItem)
                .AutoInclude();
            modelBuilder.Entity<Character>()
                .Navigation(c => c.Inventory)
                .AutoInclude();

            modelBuilder.Entity<Battle>()
                .Navigation(b => b.Opponent1)
                .AutoInclude();
            modelBuilder.Entity<Battle>()
                .Navigation(b => b.Opponent2)
                .AutoInclude();
            modelBuilder.Entity<Battle>()
                .Navigation(b => b.LastMove)
                .AutoInclude();

            modelBuilder.Entity<Battle>()
                .Navigation(b => b.LastMove)
                .AutoInclude();
        }
    }
}
