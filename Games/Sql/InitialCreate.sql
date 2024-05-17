CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "CharacterRaces" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CharacterRaces" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Skill" INTEGER NOT NULL
);

CREATE TABLE "Items" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Items" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Value" INTEGER NOT NULL,
    "Capacity" INTEGER NOT NULL,
    "Category" INTEGER NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "BattleMoves" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_BattleMoves" PRIMARY KEY AUTOINCREMENT,
    "BattleId" INTEGER NOT NULL,
    "OpponentId" INTEGER NOT NULL,
    "Move" INTEGER NOT NULL,
    "CreateDate" TEXT NOT NULL,
    CONSTRAINT "FK_BattleMoves_Battles_BattleId" FOREIGN KEY ("BattleId") REFERENCES "Battles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Battles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Battles" PRIMARY KEY AUTOINCREMENT,
    "Opponent1Id" INTEGER NOT NULL,
    "Opponent2Id" INTEGER NOT NULL,
    "Active" INTEGER NOT NULL,
    "LastMoveId" INTEGER NULL,
    "StartDate" TEXT NOT NULL,
    "EndDate" TEXT NOT NULL,
    CONSTRAINT "FK_Battles_BattleMoves_LastMoveId" FOREIGN KEY ("LastMoveId") REFERENCES "BattleMoves" ("Id"),
    CONSTRAINT "FK_Battles_Characters_Opponent1Id" FOREIGN KEY ("Opponent1Id") REFERENCES "Characters" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Battles_Characters_Opponent2Id" FOREIGN KEY ("Opponent2Id") REFERENCES "Characters" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CharacterItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_CharacterItems" PRIMARY KEY AUTOINCREMENT,
    "CharacterId" INTEGER NOT NULL,
    "ItemId" INTEGER NOT NULL,
    CONSTRAINT "FK_CharacterItems_Items_ItemId" FOREIGN KEY ("ItemId") REFERENCES "Items" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CharacterItems_Characters_CharacterId" FOREIGN KEY ("CharacterId") REFERENCES "Characters" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Characters" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Characters" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "RaceId" INTEGER NOT NULL,
    "Health" INTEGER NOT NULL,
    "Level" INTEGER NOT NULL,
    "PrimaryItemId" INTEGER NULL,
    "SecondaryItemId" INTEGER NULL,
    "InBattle" INTEGER NOT NULL,
    "CreateDate" TEXT NOT NULL,
    "OwnerId" TEXT NOT NULL,
    CONSTRAINT "FK_Characters_CharacterItems_PrimaryItemId" FOREIGN KEY ("PrimaryItemId") REFERENCES "CharacterItems" ("Id"),
    CONSTRAINT "FK_Characters_CharacterItems_SecondaryItemId" FOREIGN KEY ("SecondaryItemId") REFERENCES "CharacterItems" ("Id"),
    CONSTRAINT "FK_Characters_CharacterRaces_RaceId" FOREIGN KEY ("RaceId") REFERENCES "CharacterRaces" ("Id") ON DELETE CASCADE
);

INSERT INTO "AspNetRoles" ("Id", "ConcurrencyStamp", "Name", "NormalizedName")
VALUES ('14152646-3736-4e5d-bcbf-56c7fd3a093b', 'e1ead184-b442-4a22-a2f8-202f135cf51c', 'Admin', 'ADMIN');
SELECT changes();


INSERT INTO "AspNetUsers" ("Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName")
VALUES ('65184ed6-de49-4fbb-9ac2-c0f43f789925', 0, '901522a5-e428-417c-bf9c-51d6c539f406', 'admin@example.com', 1, 0, NULL, 'ADMIN@EXAMPLE.COM', 'ADMIN', 'AQAAAAIAAYagAAAAENd48cwhAAXtHS4/+GsO1mjw70qF0qBXNHFO5vDMFsOR3qzAzk73I/a6aG6CKhyulA==', NULL, 0, 'f5aa79a5-953e-4866-a684-50fcc3435479', 0, 'admin');
SELECT changes();


INSERT INTO "CharacterRaces" ("Id", "Name", "Skill")
VALUES (1, 'Human', 3);
SELECT changes();

INSERT INTO "CharacterRaces" ("Id", "Name", "Skill")
VALUES (2, 'Elf', 4);
SELECT changes();

INSERT INTO "CharacterRaces" ("Id", "Name", "Skill")
VALUES (3, 'Hobbit', 1);
SELECT changes();

INSERT INTO "CharacterRaces" ("Id", "Name", "Skill")
VALUES (4, 'Dwarf', 2);
SELECT changes();

INSERT INTO "CharacterRaces" ("Id", "Name", "Skill")
VALUES (5, 'Orc', 2);
SELECT changes();

INSERT INTO "CharacterRaces" ("Id", "Name", "Skill")
VALUES (6, 'Ent', 1);
SELECT changes();

INSERT INTO "CharacterRaces" ("Id", "Name", "Skill")
VALUES (7, 'Troll', 1);
SELECT changes();


INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (1, 0, 1, 'Sword', 10);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (2, 0, 1, 'Bow', 5);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (3, 0, 2, 'Shield', 10);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (4, 3, 2, 'Armor', 10);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (5, 0, 3, 'Food', 10);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (6, 0, 3, 'First Aid', 25);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (7, 5, 0, 'Small Case', 0);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (8, 10, 0, 'Medium Case', 0);
SELECT changes();

INSERT INTO "Items" ("Id", "Capacity", "Category", "Name", "Value")
VALUES (9, 20, 0, 'Large Case', 0);
SELECT changes();


INSERT INTO "AspNetUserRoles" ("RoleId", "UserId")
VALUES ('14152646-3736-4e5d-bcbf-56c7fd3a093b', '65184ed6-de49-4fbb-9ac2-c0f43f789925');
SELECT changes();


CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE INDEX "IX_BattleMoves_BattleId" ON "BattleMoves" ("BattleId");

CREATE INDEX "IX_Battles_LastMoveId" ON "Battles" ("LastMoveId");

CREATE INDEX "IX_Battles_Opponent1Id" ON "Battles" ("Opponent1Id");

CREATE INDEX "IX_Battles_Opponent2Id" ON "Battles" ("Opponent2Id");

CREATE INDEX "IX_CharacterItems_CharacterId" ON "CharacterItems" ("CharacterId");

CREATE INDEX "IX_CharacterItems_ItemId" ON "CharacterItems" ("ItemId");

CREATE UNIQUE INDEX "IX_Characters_OwnerId_Name" ON "Characters" ("OwnerId", "Name");

CREATE INDEX "IX_Characters_PrimaryItemId" ON "Characters" ("PrimaryItemId");

CREATE INDEX "IX_Characters_RaceId" ON "Characters" ("RaceId");

CREATE INDEX "IX_Characters_SecondaryItemId" ON "Characters" ("SecondaryItemId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20240517155938_InitialCreate', '8.0.4');

COMMIT;

