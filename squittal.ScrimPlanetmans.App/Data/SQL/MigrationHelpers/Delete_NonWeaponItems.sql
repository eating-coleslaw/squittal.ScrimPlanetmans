USE [PlanetmansDbContext]
GO

SET NOCOUNT ON;

IF EXISTS ( SELECT *
              FROM INFORMATION_SCHEMA.TABLES
              WHERE TABLE_NAME = N'Item' )
BEGIN

DELETE FROM [dbo].[Item]
  WHERE ItemCategoryId IN ( 99, 101, 103, 105, 106, 107, 108, 133, 134, 135, 136, 137, 139, 140, 141, 142, 143, 145, 146, 148 )
     OR ItemCategoryId IS NULL;

END