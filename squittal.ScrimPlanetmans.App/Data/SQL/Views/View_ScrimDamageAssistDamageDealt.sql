USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimDamageAssistDamageDealt'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimDamageAssistDamageDealt as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimDamageAssistDamageDealt AS
-- CREATE OR ALTER VIEW View_ScrimDamageAssistDamageDealt AS

  SELECT damages.ScrimMatchId,
         damages.ScrimMatchRound,
         damages.Timestamp,
         damages.AttackerCharacterId,
         damages.AttackerTeamOrdinal,
         damages.AttackerLoadoutId,
         damages.VictimCharacterId,
         deaths.VictimLoadoutId,
         damages.VictimTeamOrdinal,
         damages.ActionType,
         damages.ExperienceGainId,
         damages.ExperienceGainAmount,
         CASE WHEN damages.ExperienceGainId = 2 AND damages.ExperienceGainAmount > 100 THEN 100
               WHEN damages.ExperienceGainId = 371 AND damages.ExperienceGainAmount > 150 THEN 150
               WHEN damages.ExperienceGainId = 372 AND damages.ExperienceGainAmount > 300 THEN 300
               ELSE damages.ExperienceGainAmount END AdjExpAmount,
         CASE WHEN damages.ExperienceGainId = 2 THEN
                 CASE WHEN damages.ExperienceGainAmount > 100 THEN 1.0
                     ELSE damages.ExperienceGainAmount / 100.0 END
               WHEN damages.ExperienceGainId = 371 THEN 
                 CASE WHEN damages.ExperienceGainAmount > 150 THEN 1.0
                     ELSE damages.ExperienceGainAmount / 150.0 END
               WHEN damages.ExperienceGainId = 372 THEN
                 CASE WHEN damages.ExperienceGainAmount > 300 THEN 1.0
                     ELSE damages.ExperienceGainAmount / 300.0 END
               ELSE 0 END RelativeAdjExpAmount,
         CASE WHEN damages.ExperienceGainId = 2 THEN
                 CASE WHEN damages.ExperienceGainAmount > 100 THEN
                       CASE WHEN deaths.VictimLoadoutId IN ( 1, 8, 15 ) THEN 899.0
                             ELSE 999.0 END
                     ELSE CASE WHEN deaths.VictimLoadoutId IN ( 1, 8, 15 ) THEN 899.0 * damages.ExperienceGainAmount / 100.0
                               ELSE 999.0 * damages.ExperienceGainAmount / 100.0 END END
               WHEN damages.ExperienceGainId = 371 THEN 
                 CASE WHEN damages.ExperienceGainAmount > 150 THEN
                       CASE WHEN deaths.VictimLoadoutId IN ( 1, 8, 15 ) THEN 899.0
                             ELSE 999.0 END
                     ELSE CASE WHEN deaths.VictimLoadoutId IN ( 1, 8, 15 ) THEN 899.0 * damages.ExperienceGainAmount / 150.0
                               ELSE 999.0 * damages.ExperienceGainAmount / 150.0 END END
               WHEN damages.ExperienceGainId = 372 THEN
                 CASE WHEN damages.ExperienceGainAmount > 300 THEN
                       CASE WHEN deaths.VictimLoadoutId IN ( 1, 8, 15 ) THEN 899.0
                             ELSE 999.0 END
                     ELSE CASE WHEN deaths.VictimLoadoutId IN ( 1, 8, 15 ) THEN 899.0 * damages.ExperienceGainAmount / 300.0
                               ELSE 999.0 * damages.ExperienceGainAmount / 300.0 END END
               ELSE 0 END AdjustedAssistDamageDealt
    FROM ScrimDamageAssist damages
      INNER JOIN ScrimDeath deaths
        ON damages.ScrimMatchId = deaths.ScrimMatchId
          AND damages.ScrimMatchRound = deaths.ScrimMatchRound
          AND damages.Timestamp = deaths.Timestamp
          AND damages.VictimCharacterId = deaths.VictimCharacterId
    -- WHERE damages.ActionType = 304