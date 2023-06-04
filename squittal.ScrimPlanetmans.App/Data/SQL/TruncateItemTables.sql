USE [PlanetmansDbContext];

DECLARE @vMessage nvarchar(150);

BEGIN
SET @vMessage = N'Truncating item tables';
RAISERROR( @vMessage, 0, 1 ) WITH NOWAIT;

-- Items
  BEGIN
  RAISERROR( N'Started deleting Items', 0, 1 ) WITH NOWAIT;
  DELETE FROM Item;
  RAISERROR( N'Finished deleting Items', 0, 1 ) WITH NOWAIT;
  END;

  -- Item Categories
  BEGIN
  RAISERROR( N'Started deleting Item Category data', 0, 1 ) WITH NOWAIT;
  DELETE FROM ItemCategory;
  RAISERROR( N'Finished deleting Item Category data', 0, 1 ) WITH NOWAIT;
  END;

SET @vMessage = N'Finished truncating item tables';
RAISERROR( @vMessage, 0, 1 ) WITH NOWAIT;

END;