USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchReportInfantryDeaths'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchReportInfantryDeaths as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchReportInfantryDeaths AS
-- CREATE OR ALTER VIEW View_ScrimMatchReportInfantryDeaths AS

  SELECT deaths.ScrimMatchId,
       deaths.Timestamp,
       deaths.AttackerCharacterId,
       deaths.VictimCharacterId,
       MAX(COALESCE(deaths.ScrimMatchRound, 0)) ScrimMatchRound,
       MAX(COALESCE(deaths.Points, 0)) Points,
       MAX(deaths.ActionType) ActionType,
       MAX(deaths.DeathType) DeathType,
       MAX(deaths.AttackerTeamOrdinal) AttackerTeamOrdinal,
       MAX(deaths.VictimTeamOrdinal) VictimTeamOrdinal,
       MAX(CASE WHEN deaths.AttackerCharacterId = match_players.CharacterId THEN match_players.NameDisplay ELSE NULL END) AttackerNameDisplay,
       MAX(CASE WHEN deaths.VictimCharacterId = match_players.CharacterId THEN match_players.NameDisplay ELSE NULL END) VictimNameDisplay,
       MAX(deaths.AttackerNameFull) AttackerNameFull,
       MAX(deaths.VictimNameFull) VictimNameFull,
       MAX(COALESCE(deaths.AttackerFactionId, 0)) AttackerFactionId,
       MAX(COALESCE(deaths.VictimFactionId, 0)) VictimFactionId,
       MAX(COALESCE(deaths.AttackerLoadoutId, 0 ) ) AttackerLoadoutId,
       MAX(COALESCE(deaths.VictimLoadoutId, 0 ) ) VictimLoadoutId,
       MAX(CASE WHEN deaths.IsHeadshot = 1 THEN 1 ELSE 0 END) IsHeadshot,
       MAX(COALESCE(deaths.WeaponId, 0 ) ) WeaponId,
       MAX(COALESCE(weapons.Name, 'Unknown Weapon') ) WeaponName,
       MAX(deaths.ZoneId) ZoneId,
       MAX(deaths.WorldId) WorldId,
       MAX(COALESCE(damage_sums.DamageAssists, 0 ) ) DamageAssists,
       MAX(COALESCE(grenade_sums.GrenadeAssists, 0) ) GrenadeAssists,
       MAX(COALESCE(grenade_sums.ConcussionGrenadeAssists, 0) ) ConcussionGrenadeAssists,
       MAX(COALESCE(grenade_sums.EmpGrenadeAssists, 0)) EmpGrenadeAssists,
       MAX(COALESCE(grenade_sums.FlashGrenadeAssists, 0)) FlashGrenadeAssists,
       MAX(COALESCE(spot_sums.SpotAssists, 0) ) SpotAssists,
       CAST( MAX( CASE WHEN deaths.AttackerLoadoutId IN ( 3, 10, 17) AND deaths.VictimLoadoutId IN ( 3, 10, 17)
                         THEN CASE WHEN deaths.NextEventTimeDiff >= 9 AND deaths.PrevEventTimeDiff > 6 THEN 1
                                   ELSE 0 END
                       WHEN deaths.NextEventTimeDiff >= 6 AND deaths.PrevEventTimeDiff > 3 THEN 1
                       ELSE 0 END ) AS BIT ) IsTrickleDeath,
       MAX( deaths.NextEventTimeDiff ) SecondsToNextDeathEvent,
       MAX( deaths.PrevEventTimeDiff ) SecondsFromPreviousDeathEvent
    FROM ( SELECT ScrimMatchId,
                  ScrimMatchRound,
                  Timestamp,
                  ActionType,
                  DeathType,
                  AttackerTeamOrdinal,
                  AttackerCharacterId,
                  AttackerNameFull,
                  AttackerLoadoutId,
                  AttackerFactionId,
                  VictimTeamOrdinal,
                  VictimCharacterId,
                  VictimNameFull,
                  VictimLoadoutId,
                  VictimFactionId,
                  DATEDIFF(SECOND, Timestamp, LAG( Timestamp ) OVER ( PARTITION BY ScrimMatchId, ScrimMatchRound ORDER BY Timestamp DESC ) ) NextEventTimeDiff,
                  DATEDIFF(SECOND, LEAD( Timestamp ) OVER ( PARTITION BY ScrimMatchId, ScrimMatchRound ORDER BY Timestamp DESC ), Timestamp ) PrevEventTimeDiff,
                  WeaponId,
                  IsHeadshot,
                  Points,
                  ZoneId,
                  WorldId
            FROM [dbo].ScrimDeath ) deaths
      LEFT OUTER JOIN ( SELECT damages.ScrimMatchId,
                                damages.Timestamp,
                                damages.VictimCharacterId,
                                SUM( CASE WHEN damages.ActionType = 304 THEN 1 ELSE 0 END ) DamageAssists,
                                SUM( CASE WHEN damages.ActionType = 310 THEN 1 ELSE 0 END ) DamageTeamAssists,
                                SUM( CASE WHEN damages.ActionType = 312 THEN 1 ELSE 0 END ) DamageSelfAssists
                          FROM [dbo].ScrimDamageAssist damages
                          GROUP BY damages.ScrimMatchId, damages.Timestamp, damages.VictimCharacterId ) damage_sums
                ON damage_sums.ScrimMatchId = deaths.ScrimMatchId
                    AND damage_sums.Timestamp = deaths.Timestamp
                    AND damage_sums.VictimCharacterId = deaths.VictimCharacterId
      LEFT OUTER JOIN ( SELECT grenades.ScrimMatchId,
                                grenades.Timestamp,
                                grenades.VictimCharacterId,
                                SUM( CASE WHEN grenades.ActionType = 306 THEN 1 ELSE 0 END ) GrenadeAssists,
                                SUM( CASE WHEN grenades.ActionType = 311 THEN 1 ELSE 0 END ) GrenadeTeamAssists,
                                SUM( CASE WHEN grenades.ActionType = 313 THEN 1 ELSE 0 END ) GrenadeSelfAssists,
                                SUM( CASE WHEN grenades.ActionType = 306 AND grenades.ExperienceGainId IN ( 550, 551 ) THEN 1 ELSE 0 END ) ConcussionGrenadeAssists,
                                SUM( CASE WHEN grenades.ActionType = 311 AND grenades.ExperienceGainId IN ( 550, 551 ) THEN 1 ELSE 0 END ) ConcussionGrenadeTeamAssists,
                                SUM( CASE WHEN grenades.ActionType = 313 AND grenades.ExperienceGainId IN ( 550, 551 ) THEN 1 ELSE 0 END ) ConcussionGrenadeSelfAssists,
                                SUM( CASE WHEN grenades.ActionType = 306 AND grenades.ExperienceGainId IN ( 553, 553 ) THEN 1 ELSE 0 END ) EmpGrenadeAssists,
                                SUM( CASE WHEN grenades.ActionType = 311 AND grenades.ExperienceGainId IN ( 553, 553 ) THEN 1 ELSE 0 END ) EmpGrenadeTeamAssists,
                                SUM( CASE WHEN grenades.ActionType = 313 AND grenades.ExperienceGainId IN ( 553, 553 ) THEN 1 ELSE 0 END ) EmpGrenadeSelfAssists,
                                SUM( CASE WHEN grenades.ActionType = 306 AND grenades.ExperienceGainId IN ( 554, 555 ) THEN 1 ELSE 0 END ) FlashGrenadeAssists,
                                SUM( CASE WHEN grenades.ActionType = 311 AND grenades.ExperienceGainId IN ( 554, 555 ) THEN 1 ELSE 0 END ) FlashGrenadeTeamAssists,
                                SUM( CASE WHEN grenades.ActionType = 313 AND grenades.ExperienceGainId IN ( 554, 555 ) THEN 1 ELSE 0 END ) FlashGrenadeSelfAssists
                          FROM [dbo].ScrimGrenadeAssist grenades
                          GROUP BY grenades.ScrimMatchId, grenades.Timestamp, grenades.VictimCharacterId ) grenade_sums
                ON grenade_sums.ScrimMatchId = deaths.ScrimMatchId
                    AND grenade_sums.Timestamp = deaths.Timestamp
                    AND grenade_sums.VictimCharacterId = deaths.VictimCharacterId
      LEFT OUTER JOIN ( SELECT spots.ScrimMatchId,
                                spots.Timestamp,
                                spots.VictimCharacterId,
                                SUM( CASE WHEN spots.ActionType = 308 THEN 1 ELSE 0 END ) SpotAssists
                          FROM [dbo].ScrimSpotAssist spots
                          GROUP BY spots.ScrimMatchId, spots.Timestamp, spots.VictimCharacterId ) spot_sums
                ON spot_sums.ScrimMatchId = deaths.ScrimMatchId
                    AND spot_sums.Timestamp = deaths.Timestamp
                    AND spot_sums.VictimCharacterId = deaths.VictimCharacterId
      LEFT OUTER JOIN [dbo].ScrimMatchParticipatingPlayer match_players
        ON match_players.ScrimMatchId = deaths.ScrimMatchId
            AND ( match_players.CharacterId = deaths.AttackerCharacterId
                  OR match_players.CharacterId = deaths.VictimCharacterId )
      LEFT OUTER JOIN [dbo].Item weapons
        ON weapons.Id = deaths.WeaponId
      GROUP BY deaths.ScrimMatchId, deaths.Timestamp, deaths.AttackerCharacterId, deaths.VictimCharacterId
