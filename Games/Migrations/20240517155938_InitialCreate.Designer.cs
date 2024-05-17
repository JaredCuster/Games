﻿// <auto-generated />
using System;
using Games.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Games.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240517155938_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.4");

            modelBuilder.Entity("Games.Models.Battle", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("TEXT");

                    b.Property<int?>("LastMoveId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Opponent1Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Opponent2Id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LastMoveId");

                    b.HasIndex("Opponent1Id");

                    b.HasIndex("Opponent2Id");

                    b.ToTable("Battles");
                });

            modelBuilder.Entity("Games.Models.BattleMove", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BattleId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Move")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OpponentId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("BattleId");

                    b.ToTable("BattleMoves");
                });

            modelBuilder.Entity("Games.Models.Character", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("Health")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("InBattle")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Level")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("PrimaryItemId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RaceId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SecondaryItemId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PrimaryItemId");

                    b.HasIndex("RaceId");

                    b.HasIndex("SecondaryItemId");

                    b.HasIndex("OwnerId", "Name")
                        .IsUnique();

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Games.Models.CharacterItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CharacterId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ItemId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("CharacterId");

                    b.HasIndex("ItemId");

                    b.ToTable("CharacterItems");
                });

            modelBuilder.Entity("Games.Models.CharacterRace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Skill")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("CharacterRaces");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Human",
                            Skill = 3
                        },
                        new
                        {
                            Id = 2,
                            Name = "Elf",
                            Skill = 4
                        },
                        new
                        {
                            Id = 3,
                            Name = "Hobbit",
                            Skill = 1
                        },
                        new
                        {
                            Id = 4,
                            Name = "Dwarf",
                            Skill = 2
                        },
                        new
                        {
                            Id = 5,
                            Name = "Orc",
                            Skill = 2
                        },
                        new
                        {
                            Id = 6,
                            Name = "Ent",
                            Skill = 1
                        },
                        new
                        {
                            Id = 7,
                            Name = "Troll",
                            Skill = 1
                        });
                });

            modelBuilder.Entity("Games.Models.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Capacity")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Category")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Items");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Capacity = 0,
                            Category = 1,
                            Name = "Sword",
                            Value = 10
                        },
                        new
                        {
                            Id = 2,
                            Capacity = 0,
                            Category = 1,
                            Name = "Bow",
                            Value = 5
                        },
                        new
                        {
                            Id = 3,
                            Capacity = 0,
                            Category = 2,
                            Name = "Shield",
                            Value = 10
                        },
                        new
                        {
                            Id = 4,
                            Capacity = 3,
                            Category = 2,
                            Name = "Armor",
                            Value = 10
                        },
                        new
                        {
                            Id = 5,
                            Capacity = 0,
                            Category = 3,
                            Name = "Food",
                            Value = 10
                        },
                        new
                        {
                            Id = 6,
                            Capacity = 0,
                            Category = 3,
                            Name = "First Aid",
                            Value = 25
                        },
                        new
                        {
                            Id = 7,
                            Capacity = 5,
                            Category = 0,
                            Name = "Small Case",
                            Value = 0
                        },
                        new
                        {
                            Id = 8,
                            Capacity = 10,
                            Category = 0,
                            Name = "Medium Case",
                            Value = 0
                        },
                        new
                        {
                            Id = 9,
                            Capacity = 20,
                            Category = 0,
                            Name = "Large Case",
                            Value = 0
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "14152646-3736-4e5d-bcbf-56c7fd3a093b",
                            ConcurrencyStamp = "e1ead184-b442-4a22-a2f8-202f135cf51c",
                            Name = "Admin",
                            NormalizedName = "ADMIN"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "65184ed6-de49-4fbb-9ac2-c0f43f789925",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "901522a5-e428-417c-bf9c-51d6c539f406",
                            Email = "admin@example.com",
                            EmailConfirmed = true,
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@EXAMPLE.COM",
                            NormalizedUserName = "ADMIN",
                            PasswordHash = "AQAAAAIAAYagAAAAENd48cwhAAXtHS4/+GsO1mjw70qF0qBXNHFO5vDMFsOR3qzAzk73I/a6aG6CKhyulA==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "f5aa79a5-953e-4866-a684-50fcc3435479",
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ClaimType")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RoleId")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "65184ed6-de49-4fbb-9ac2-c0f43f789925",
                            RoleId = "14152646-3736-4e5d-bcbf-56c7fd3a093b"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Games.Models.Battle", b =>
                {
                    b.HasOne("Games.Models.BattleMove", "LastMove")
                        .WithMany()
                        .HasForeignKey("LastMoveId");

                    b.HasOne("Games.Models.Character", "Opponent1")
                        .WithMany()
                        .HasForeignKey("Opponent1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Games.Models.Character", "Opponent2")
                        .WithMany()
                        .HasForeignKey("Opponent2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LastMove");

                    b.Navigation("Opponent1");

                    b.Navigation("Opponent2");
                });

            modelBuilder.Entity("Games.Models.BattleMove", b =>
                {
                    b.HasOne("Games.Models.Battle", null)
                        .WithMany("Moves")
                        .HasForeignKey("BattleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Games.Models.Character", b =>
                {
                    b.HasOne("Games.Models.CharacterItem", "PrimaryItem")
                        .WithMany()
                        .HasForeignKey("PrimaryItemId");

                    b.HasOne("Games.Models.CharacterRace", "Race")
                        .WithMany()
                        .HasForeignKey("RaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Games.Models.CharacterItem", "SecondaryItem")
                        .WithMany()
                        .HasForeignKey("SecondaryItemId");

                    b.Navigation("PrimaryItem");

                    b.Navigation("Race");

                    b.Navigation("SecondaryItem");
                });

            modelBuilder.Entity("Games.Models.CharacterItem", b =>
                {
                    b.HasOne("Games.Models.Character", null)
                        .WithMany("Inventory")
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Games.Models.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Games.Models.Battle", b =>
                {
                    b.Navigation("Moves");
                });

            modelBuilder.Entity("Games.Models.Character", b =>
                {
                    b.Navigation("Inventory");
                });
#pragma warning restore 612, 618
        }
    }
}