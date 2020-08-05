USE [PlanetmansDbContext];

GO

CREATE OR ALTER VIEW View_ScrimMatchInfo AS

SELECT config1.ScrimMatchId,
       MAX(StartTime) StartTime,
       MAX(config1.Title) Title,
       MAX(config1.ScrimMatchRound) RoundCount,
       MAX(world.Id) WorldId,
       MAX(world.Name) WorldName,
       MAX(facility.FacilityId) FacilityId,
       MAX(facility.FacilityName) FacilityName
  FROM ScrimMatchRoundConfiguration config1
    INNER JOIN ScrimMatchRoundConfiguration config2
      ON ( config1.ScrimMatchId = config2.ScrimMatchId
           AND config1.ScrimMatchRound >= config2.ScrimMatchRound )
    INNER JOIN ScrimMatch match
      ON config1.ScrimMatchId = match.Id
    INNER JOIN World world
      ON config1.WorldId = world.Id
    LEFT OUTER JOIN MapRegion facility
      ON config1.FacilityId = facility.FacilityId
  WHERE config1.ScrimMatchRound >= config2.ScrimMatchRound
  GROUP BY config1.ScrimMatchId;