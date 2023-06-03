USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchInfo'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchInfo as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchInfo AS
-- CREATE OR ALTER VIEW View_ScrimMatchInfo AS

SELECT config1.ScrimMatchId,
       MAX(StartTime) StartTime,
       MAX(config1.Title) Title,
       MAX(config1.ScrimMatchRound) RoundCount,
       MAX(ruleset.Id) RulesetId,
       MAX(ruleset.Name) RulesetName,
       MAX(world.Id) WorldId,
       MAX(world.Name) WorldName,
       MAX(facility.FacilityId) FacilityId,
       MAX(facility.FacilityName) FacilityName,
       MAX( CASE WHEN team_factions.TeamOrdinal = 1 THEN team_factions.FactionId ELSE 0 END ) TeamOneFactionId,
       MAX( CASE WHEN team_factions.TeamOrdinal = 2 THEN team_factions.FactionId ELSE 0 END ) TeamTwoFactionId
  FROM [dbo].ScrimMatchRoundConfiguration config1
    INNER JOIN [dbo].ScrimMatchRoundConfiguration config2
      ON ( config1.ScrimMatchId = config2.ScrimMatchId
           AND config1.ScrimMatchRound >= config2.ScrimMatchRound )
    INNER JOIN [dbo].ScrimMatch match
      ON config1.ScrimMatchId = match.Id
    INNER JOIN [dbo].Ruleset Ruleset
      ON match.RulesetId = ruleset.Id
    INNER JOIN [dbo].World world
      ON config1.WorldId = world.Id
    LEFT OUTER JOIN [dbo].MapRegion facility
      ON config1.FacilityId = facility.FacilityId
    LEFT OUTER JOIN ( SELECT match_players.ScrimMatchId,
                             match_players.TeamOrdinal,
                             MAX(match_players.FactionId) FactionId
                        FROM [dbo].ScrimMatchParticipatingPlayer match_players
                        WHERE match_players.TeamOrdinal IN ( 1, 2 )
                        GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal ) team_factions
      ON config1.ScrimMatchId = team_factions.ScrimMatchId
  WHERE config1.ScrimMatchRound >= config2.ScrimMatchRound
  GROUP BY config1.ScrimMatchId;