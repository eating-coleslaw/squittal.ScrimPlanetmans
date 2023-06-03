USE [PlanetmansDbContext];

SET NOCOUNT ON;

DECLARE @iReviveInfantryType      int = 300,
        @iReviveMaxType           int = 301,
        @iEnemyReviveInfantryType int = 302,
        @iEnemyReviveMaxType      int = 303,
        @iUnknownType             int = 9001;

-- Column exists and caller has permission to view the object
If COL_LENGTH('ScrimRevive', 'EnemyActionType') IS NOT NULL
BEGIN
  UPDATE ScrimRevive
    SET EnemyActionType = IIF(ActionType = @iReviveMaxType, @iEnemyReviveMaxType, @iEnemyReviveInfantryType)
    WHERE EnemyActionType = @iUnknownType;
END;

SET NOCOUNT OFF;