USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'World' )
BEGIN
    
  CREATE TABLE #Staging_World
    ( Id   int NOT NULL,
      Name nvarchar(max) );

  INSERT INTO #Staging_World VALUES
    (1, N'Connery'),
    (10, N'Miller'),
    (13, N'Cobalt'),
    (17, N'Emerald'),
    (19, N'Jaeger'),
    (25, N'Briggs'),
    (40, N'SolTech');

  MERGE [dbo].[World] as target
    USING ( SELECT Id, Name FROM #Staging_World ) as source
      ON ( target.Id = source.Id )
    WHEN MATCHED THEN
      UPDATE SET Name = source.Name
    WHEN NOT MATCHED THEN
      INSERT ( [Id], [Name] )
      VALUES ( source.Id, source.Name );
  
  DROP TABLE #Staging_World;

END;