USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'ItemCategory' )
BEGIN
    
  CREATE TABLE #Staging_ItemCategory
    ( Id               int NOT NULL,
      Name             nvarchar(max) NULL,
      Domain           int NOT NULL,
      IsWeaponCategory bit not NULL );

  INSERT INTO #Staging_ItemCategory VALUES
    (2, N'Knife', 1, 1),
    (3, N'Pistol', 1, 1),
    (4, N'Shotgun', 1, 1),
    (5, N'SMG', 1, 1),
    (6, N'LMG', 1, 1),
    (7, N'Assault Rifle', 1, 1),
    (8, N'Carbine', 1, 1),
    (9, N'AV MAX (Left)', 2, 1),
    (10, N'AI MAX (Left)', 2, 1),
    (11, N'Sniper Rifle', 1, 1),
    (12, N'Scout Rifle', 1, 1),
    (13, N'Rocket Launcher', 1, 1),
    (14, N'Heavy Weapon', 1, 1),
    (15, N'Flamethrower MAX', 6, 1),
    (16, N'Flak MAX', 2, 1),
    (17, N'Grenade', 1, 1),
    (18, N'Explosive', 1, 1),
    (19, N'Battle Rifle', 1, 1),
    (20, N'AA MAX (Right)', 2, 1),
    (21, N'AV MAX (Right)', 2, 1),
    (22, N'AI MAX (Right)', 2, 1),
    (23, N'AA MAX (Left)', 2, 1),
    (24, N'Crossbow', 1, 1),
    (99, N'Camo', 5, 0),
    (100, N'Infantry', 1, 1),
    (101, N'Vehicles', 5, 0),
    (102, N'Infantry Weapons', 1, 1),
    (103, N'Infantry Gear', 5, 0),
    (104, N'Vehicle Weapons', 5, 1),
    (105, N'Vehicle Gear', 5, 0),
    (106, N'Armor Camo', 5, 0),
    (107, N'Weapon Camo', 5, 0),
    (108, N'Vehicle Camo', 5, 0),
    (109, N'Flash Primary Weapon', 3, 1),
    (110, N'Galaxy Left Weapon', 4, 1),
    (111, N'Galaxy Tail Weapon', 4, 1),
    (112, N'Galaxy Right Weapon', 4, 1),
    (113, N'Galaxy Top Weapon', 4, 1),
    (114, N'Harasser Top Gunner', 3, 1),
    (115, N'Liberator Belly Weapon', 4, 1),
    (116, N'Liberator Nose Cannon', 4, 1),
    (117, N'Liberator Tail Weapon', 4, 1),
    (118, N'Lightning Primary Weapon ', 3, 1),
    (119, N'Magrider Gunner Weapon', 3, 1),
    (120, N'Magrider Primary Weapon', 3, 1),
    (121, N'Mosquito Nose Cannon', 4, 1),
    (122, N'Mosquito Wing Mount', 4, 1),
    (123, N'Prowler Gunner Weapon', 3, 1),
    (124, N'Prowler Primary Weapon', 3, 1),
    (125, N'Reaver Nose Cannon', 4, 1),
    (126, N'Reaver Wing Mount', 4, 1),
    (127, N'Scythe Nose Cannon', 4, 1),
    (128, N'Scythe Wing Mount', 4, 1),
    (129, N'Sunderer Front Gunner', 3, 1),
    (130, N'Sunderer Rear Gunner', 3, 1),
    (131, N'Vanguard Gunner Weapon', 3, 1),
    (132, N'Vanguard Primary Weapon', 3, 1),
    (133, N'Implants', 5, 0),
    (134, N'Consolidated Camo', 5, 0),
    (135, N'VO Packs', 5, 0),
    (136, N'Male VO Pack', 5, 0),
    (137, N'Female VO Pack', 5, 0),
    (138, N'Valkyrie Nose Gunner', 4, 1),
    (139, N'Infantry Abilities', 5, 0),
    (140, N'Vehicle Abilities', 5, 0),
    (141, N'Boosts & Utilities', 5, 0),
    (142, N'Consolidated Decal', 5, 0),
    (143, N'Attachments', 5, 0),
    (144, N'ANT Top Turret', 3, 1),
    (145, N'ANT Utility', 5, 0),
    (147, N'Aerial Combat Weapon', 1, 1),
    (148, N'ANT Harvesting Tool', 5, 0),
    (157, N'Hybrid Rifle', 1, 1),
    (207, N'Weapon', 5, 1),
    (208, N'Bastion Point Defense', 5, 1),
    (209, N'Bastion Bombard', 5, 1),
    (210, N'Bastion Weapon System', 5, 1),
    (211, N'Colossus Primary Weapon', 5, 1),
    (212, N'Colossus Front Right Weapon', 5, 1),
    (213, N'Colossus Front Left Weapon', 5, 1),
    (214, N'Colossus Rear Right Weapon', 5, 1),
    (215, N'Colossus Rear Left Weapon', 5, 1);

  MERGE [dbo].[ItemCategory] as target
    USING ( SELECT Id, Name, Domain, IsWeaponCategory FROM #Staging_ItemCategory ) as source
      ON ( target.Id = source.Id )
    WHEN MATCHED THEN
      UPDATE SET Name = source.Name,
                 Domain = source.Domain,
                 IsWeaponCategory = source.IsWeaponCategory
    WHEN NOT MATCHED THEN
      INSERT ( [Id], [Name], [Domain], [IsWeaponCategory] )
      VALUES ( source.Id, source.Name, source.Domain, source.IsWeaponCategory );
  
  DROP TABLE #Staging_ItemCategory;

END;