USE [PlanetmansDbContext];

DECLARE @nCursorRulesetId INT;

DECLARE RULESET_CURSOR CURSOR FAST_FORWARD FOR
  SELECT ruleset.Id
    FROM Ruleset ruleset
      LEFT OUTER JOIN RulesetOverlayConfiguration overlay
        ON ruleset.Id = overlay.RulesetId
    WHERE overlay.RulesetId IS NULL

OPEN RULESET_CURSOR
FETCH NEXT FROM RULESET_CURSOR INTO @nCursorRulesetId

WHILE @@FETCH_STATUS = 0
BEGIN

  INSERT INTO RulesetOverlayConfiguration ( RulesetId, UseCompactLayout, StatsDisplayType, ShowStatusPanelScores )
    VALUES ( @nCursorRulesetId, 0, 1, null );

  FETCH NEXT FROM RULESET_CURSOR INTO @nCursorRulesetId;

END;

CLOSE RULESET_CURSOR;
DEALLOCATE RULESET_CURSOR;