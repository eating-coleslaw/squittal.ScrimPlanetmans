USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'Profile' )
BEGIN
    
  CREATE TABLE #Staging_Profile
    ( Id            int NOT NULL,
      ProfileTypeId int NOT NULL,
      FactionId     int NOT NULL,
      Name          nvarchar(max) NULL,
      ImageId       nvarchar(max) NULL );

  INSERT INTO #Staging_Profile VALUES
    (2, 1, 2, N'Infiltrator', 204),
    (4, 3, 2, N'Light Assault', 62),
    (5, 4, 2, N'Combat Medic', 65),
    (6, 5, 2, N'Engineer', 201),
    (7, 6, 2, N'Heavy Assault', 59),
    (8, 7, 2, N'MAX', 207),
    (10, 1, 3, N'Infiltrator', 204),
    (12, 3, 3, N'Light Assault', 62),
    (13, 4, 3, N'Combat Medic', 65),
    (14, 5, 3, N'Engineer', 201),
    (15, 6, 3, N'Heavy Assault', 59),
    (16, 7, 3, N'MAX', 207),
    (17, 1, 1, N'Infiltrator', 204),
    (19, 3, 1, N'Light Assault', 62),
    (20, 4, 1, N'Combat Medic', 65),
    (21, 5, 1, N'Engineer', 201),
    (22, 6, 1, N'Heavy Assault', 59),
    (23, 7, 1, N'MAX', 207),
    (190, 1, 4, N'Infiltrator', 204),
    (191, 3, 4, N'Light Assault', 62),
    (192, 4, 4, N'Combat Medic', 65),
    (193, 5, 4, N'Engineer', 201),
    (194, 6, 4, N'Heavy Assault', 59),
    (252, 7, 4, N'Defector', 207);

  MERGE [dbo].[Profile] as target
    USING ( SELECT Id, ProfileTypeId, FactionId, Name, ImageId FROM #Staging_Profile ) as source
      ON ( target.Id = source.Id )
    WHEN MATCHED THEN
      UPDATE SET ProfileTypeId = source.ProfileTypeId,
                 FactionId = source.FactionId,
                 Name = source.Name,
                 ImageId = source.ImageId
    WHEN NOT MATCHED THEN
      INSERT ( [Id], [ProfileTypeId], [FactionId], [Name], [ImageId] )
      VALUES ( source.Id, source.ProfileTypeId, source.FactionId, source.Name, source.ImageId );
  
  DROP TABLE #Staging_Profile;

END;