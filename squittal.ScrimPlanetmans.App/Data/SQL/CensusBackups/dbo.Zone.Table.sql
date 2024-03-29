USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'Zone' )
BEGIN
    
  CREATE TABLE #Staging_Zone
    ( Id          int NOT NULL,
      Name        nvarchar(max) NULL,
      Description nvarchar(max) NULL,
      Code        nvarchar(max) NULL,
      HexSize     int NULL );

  INSERT INTO #Staging_Zone VALUES
    (2, N'Indar', N'The arid continent of Indar is home to multiple biomes, providing unique challenges for its combatants.', N'Indar', 200),
    (4, N'Hossin', N'Hossin''s dense mangrove and willow forests provide air cover along its many swamps and highlands.', N'Hossin', 200),
    (6, N'Amerish', N'Amerish''s lush groves and rocky outcroppings provide ample cover between its rolling plains and mountain passes.', N'Amerish', 200),
    (8, N'Esamir', N'Esamir''s expanses of frigid tundra and craggy mountains provide little cover from airborne threats.', N'Esamir', 200),
    (96, N'VR Training', N'Experiment with all weapons, vehicles and attachments in your empire''s own VR Training simulator.', N'VR training zone (NC)', 335),
    (97, N'VR Training', N'Experiment with all weapons, vehicles and attachments in your empire''s own VR Training simulator.', N'VR training zone (TR)', 335),
    (98, N'VR Training', N'Experiment with all weapons, vehicles and attachments in your empire''s own VR Training simulator.', N'VR training zone (VS)', 335);

  MERGE [dbo].[Zone] as target
    USING ( SELECT Id, Name, Description, Code, HexSize FROM #Staging_Zone ) as source
      ON ( target.Id = source.Id )
    WHEN MATCHED THEN
      UPDATE SET Name = source.Name,
                 Description = source.Description,
                 Code = source.Code,
                 HexSize = source.HexSize
    WHEN NOT MATCHED THEN
      INSERT ( [Id], [Name], [Description], [Code], [HexSize] )
      VALUES ( source.Id, source.Name, source.Description, source.Code, source.HexSize );
  
  DROP TABLE #Staging_Zone;

END