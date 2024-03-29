USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'Loadout' )
BEGIN
    
  CREATE TABLE #Staging_Loadout
    ( Id        int NOT NULL,
      ProfileId int NOT NULL,
      FactionId int NOT NULL,
      CodeName  nvarchar(max) NULL );

  INSERT INTO #Staging_Loadout VALUES
    (1, 2, 2, N'NC Infiltrator'),
    (3, 4, 2, N'NC Light Assault'),
    (4, 5, 2, N'NC Medic'),
    (5, 6, 2, N'NC Engineer'),
    (6, 7, 2, N'NC Heavy Assault'),
    (7, 8, 2, N'NC MAX'),
    (8, 10, 3, N'TR Infiltrator'),
    (10, 12, 3, N'TR Light Assault'),
    (11, 13, 3, N'TR Medic'),
    (12, 14, 3, N'TR Engineer'),
    (13, 15, 3, N'TR Heavy Assault'),
    (14, 16, 3, N'TR MAX'),
    (15, 17, 1, N'VS Infiltrator'),
    (17, 19, 1, N'VS Light Assault'),
    (18, 20, 1, N'VS Medic'),
    (19, 21, 1, N'VS Engineer'),
    (20, 22, 1, N'VS Heavy Assault'),
    (21, 23, 1, N'VS MAX'),
    (28, 190, 4, N'NS Infiltrator'),
    (29, 191, 4, N'NS Light Assault'),
    (30, 192, 4, N'NS Combat Medic'),
    (31, 193, 4, N'NS Engineer'),
    (32, 194, 4, N'NS Heavy Assault'),
    (45, 252, 4, N'NS Defector');

  MERGE [dbo].[Loadout] as target
    USING ( SELECT Id, ProfileId, FactionId, CodeName FROM #Staging_Loadout ) as source
      ON ( target.Id = source.Id )
    WHEN MATCHED THEN
      UPDATE SET ProfileId = source.ProfileId,
                 FactionId = source.FactionId,
                 CodeName = source.CodeName
    WHEN NOT MATCHED THEN
      INSERT ( [Id], [ProfileId], [FactionId], [CodeName] )
      VALUES ( source.Id, source.ProfileId, source.FactionId, source.CodeName );
  
  DROP TABLE #Staging_Loadout;

END;