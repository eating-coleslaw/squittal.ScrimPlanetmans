USE [PlanetmansDbContext];

SET NOCOUNT ON;

DECLARE @iDefaultScrimMatchRulesetId  int = -1,
        @iDefaultRulesetId            int =  1;

-- Column exists and caller has permission to view the object
If COL_LENGTH('ScrimMatch', 'RulesetId') IS NOT NULL
BEGIN
  UPDATE ScrimMatch
    SET RulesetId = @iDefaultRulesetId
    WHERE RulesetId = @iDefaultScrimMatchRulesetId
END;

SET NOCOUNT OFF;