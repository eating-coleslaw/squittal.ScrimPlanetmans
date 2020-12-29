USE [PlanetmansDbContext];
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT 1
              FROM INFORMATION_SCHEMA.COLUMNS columns
              WHERE columns.TABLE_NAME = 'Ruleset'
                AND columns.COLUMN_NAME = 'SourceFile' )

BEGIN

  UPDATE Ruleset
    SET SourceFile = null
    WHERE SourceFile = '';

END;
