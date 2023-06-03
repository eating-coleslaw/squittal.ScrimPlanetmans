USE [PlanetmansDbContext];

SET NOCOUNT ON;

DECLARE @iDefaultRulesetId                int = 1,
        @iInfantryKillInfantryActionType  int = 20;

-- Column exists and caller has permission to view the object
If COL_LENGTH('RulesetActionRule', 'DeferToItemCategoryRules') IS NOT NULL
BEGIN
  UPDATE RulesetActionRule
    SET DeferToItemCategoryRules = 1
    WHERE RulesetId = @iDefaultRulesetId
      AND ScrimActionType = @iInfantryKillInfantryActionType
END;

SET NOCOUNT OFF;