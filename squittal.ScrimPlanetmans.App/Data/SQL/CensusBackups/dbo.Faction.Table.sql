USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'Faction' )

BEGIN
  
  CREATE TABLE #Staging_Faction
      ( Id             int NOT NULL,
        Name           nvarchar(max) NULL,
        ImageId        nvarchar(max) NULL,
        CodeTag        int NULL,
        UserSelectable bit not NULL );

  INSERT INTO #Staging_Faction VALUES
    ( 0, N'None',             0,  N'None', 0 ),
    ( 1, N'Vanu Sovereignty', 94, N'VS',   1 ),
    ( 2, N'New Conglomerate', 12, N'NC',   1 ),
    ( 3, N'Terran Republic',  18, N'TR',   1 ),
    ( 4, N'NS Operatives',    0,  N'NSO',  1 );

  MERGE [dbo].[Faction] as target
    USING ( SELECT Id, Name, ImageId, CodeTag, UserSelectable FROM #Staging_Faction ) as source
      ON ( target.Id = source.Id )
    WHEN MATCHED THEN
      UPDATE SET Name = source.Name,
                 ImageId = source.ImageId,
                 CodeTag = source.CodeTag,
                 UserSelectable = source.UserSelectable
    WHEN NOT MATCHED THEN
      INSERT ( [Id], [Name], [ImageId], [CodeTag], [UserSelectable] )
      VALUES ( source.Id, source.Name, source.ImageId, source.CodeTag, source.UserSelectable );
  
  DROP TABLE #Staging_Faction;

END;