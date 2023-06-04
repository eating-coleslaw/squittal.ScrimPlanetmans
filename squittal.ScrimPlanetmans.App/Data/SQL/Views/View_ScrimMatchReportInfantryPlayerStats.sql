USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchReportInfantryPlayerStats'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchReportInfantryPlayerStats as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchReportInfantryPlayerStats AS
-- CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerStats AS

  SELECT DISTINCT
         match_players.ScrimMatchId,
         match_players.TeamOrdinal,
         match_players.CharacterId,
         match_players.NameDisplay,
         match_players.NameFull,
         match_players.FactionId,
         match_players.WorldId,
         match_players.PrestigeLevel,
         COALESCE(kill_sums.Points, 0) + COALESCE(revives_given_sums.Points, 0) + COALESCE(revives_undone_sums.KillsUndoneByRevivePoints, 0) as Points,
         COALESCE(kill_sums.NetScore, 0) + COALESCE(revives_given_sums.Points, 0) - COALESCE(revives_taken_sums.EnemyPoints, 0) + COALESCE(revives_undone_sums.KillsUndoneByRevivePoints, 0) as NetScore,
         COALESCE(kill_sums.Kills, 0) Kills,
         COALESCE(kill_sums.HeadshotKills, 0) HeadshotKills,
         COALESCE(kill_sums.Deaths, 0) Deaths,
         COALESCE(kill_sums.HeadshotEnemyDeaths, 0) HeadshotEnemyDeaths,
         COALESCE(kill_sums.TeamKills, 0) TeamKills,
         COALESCE(kill_sums.Suicides, 0) Suicides,
         COALESCE(kill_sums.ScoredDeaths, 0) ScoredDeaths,
         COALESCE(kill_sums.ZeroPointDeaths, 0) ZeroPointDeaths,
         COALESCE(kill_sums.TeamKillDeaths, 0) TeamKillDeaths,
         COALESCE(kill_sums.TrickleDeaths_6s, 0) TrickleDeaths,
         COALESCE(damage_sums.DamageAssists, 0) DamageAssists,
         COALESCE(damage_sums.DamageTeamAssists, 0) DamageTeamAssists,
         COALESCE(kill_sums.DamageAssistedKills, 0) DamageAssistedKills,
         COALESCE(kill_sums.DamageAssistedOnlyKills, 0) DamageAssistedOnlyKills,
         COALESCE(kill_sums.GrenadeAssistedKills, 0) GrenadeAssistedKills,
         COALESCE(kill_sums.GrenadeAssistedOnlyKills, 0) GrenadeAssistedOnlyKills,
         COALESCE(kill_sums.SpotAssistedKills, 0) SpotAssistedKills,
         COALESCE(kill_sums.SpotAssistedOnlyKills, 0) SpotAssistedOnlyKills,
         COALESCE(kill_sums.AssistedKills, 0) AssistedKills,
         COALESCE(kill_sums.DamageAssistedDeaths, 0) DamageAssistedDeaths,
         COALESCE(kill_sums.DamageAssistedOnlyDeaths, 0) DamageAssistedOnlyDeaths,
         COALESCE(kill_sums.GrenadeAssistedDeaths, 0) GrenadeAssistedDeaths,
         COALESCE(kill_sums.GrenadeAssistedOnlyDeaths, 0) GrenadeAssistedOnlyDeaths,
         COALESCE(kill_sums.SpotAssistedDeaths, 0) SpotAssistedDeaths,
         COALESCE(kill_sums.SpotAssistedOnlyDeaths, 0) SpotAssistedOnlyDeaths,
         COALESCE(kill_sums.DamageAssistedEnemyDeaths, 0) DamageAssistedEnemyDeaths,
         COALESCE(kill_sums.DamageAssistedOnlyEnemyDeaths, 0) DamageAssistedOnlyEnemyDeaths,
         COALESCE(kill_sums.GrenadeAssistedEnemyDeaths, 0) GrenadeAssistedEnemyDeaths,
         COALESCE(kill_sums.GrenadeAssistedOnlyEnemyDeaths, 0) GrenadeAssistedOnlyEnemyDeaths,
         COALESCE(kill_sums.SpotAssistedEnemyDeaths, 0) SpotAssistedEnemyDeaths,
         COALESCE(kill_sums.SpotAssistedOnlyEnemyDeaths, 0) SpotAssistedOnlyEnemyDeaths,
         COALESCE(kill_sums.UnassistedEnemyDeaths, 0) UnassistedEnemyDeaths,
         COALESCE(kill_sums.KillsAsHeavyAssault, 0) KillsAsHeavyAssault,
         COALESCE(kill_sums.KillsAsInfiltrator, 0) KillsAsInfiltrator,
         COALESCE(kill_sums.KillsAsLightAssault, 0) KillsAsLightAssault,
         COALESCE(kill_sums.KillsAsMedic, 0) KillsAsMedic,
         COALESCE(kill_sums.KillsAsEngineer, 0) KillsAsEngineer,
         COALESCE(kill_sums.KillsAsMax, 0) KillsAsMax,
         COALESCE(kill_sums.DeathsAsHeavyAssault, 0) DeathsAsHeavyAssault,
         COALESCE(kill_sums.DeathsAsInfiltrator, 0) DeathsAsInfiltrator,
         COALESCE(kill_sums.DeathsAsLightAssault, 0) DeathsAsLightAssault,
         COALESCE(kill_sums.DeathsAsMedic, 0) DeathsAsMedic,
         COALESCE(kill_sums.DeathsAsEngineer, 0) DeathsAsEngineer,
         COALESCE(kill_sums.DeathsAsMax, 0) DeathsAsMax,
         COALESCE(damage_sums.DamageAssistsAsHeavyAssault, 0) DamageAssistsAsHeavyAssault,
         COALESCE(damage_sums.DamageAssistsAsInfiltrator, 0) DamageAssistsAsInfiltrator,
         COALESCE(damage_sums.DamageAssistsAsLightAssault, 0) DamageAssistsAsLightAssault,
         COALESCE(damage_sums.DamageAssistsAsMedic, 0) DamageAssistsAsMedic,
         COALESCE(damage_sums.DamageAssistsAsEngineer, 0) DamageAssistsAsEngineer,
         COALESCE(damage_sums.DamageAssistsAsMax, 0) DamageAssistsAsMax,
         CAST(ROUND(COALESCE(kill_sums.KillDamageDealt, 0), 0) AS int) KillDamageDealt,
         CAST(ROUND(COALESCE(damage_sums.AssistDamageDealt, 0), 0) AS int) AssistDamageDealt,
         CAST(ROUND(COALESCE(kill_sums.KillDamageDealt, 0) + COALESCE(damage_sums.AssistDamageDealt, 0), 0) AS int) TotalDamageDealt,
         COALESCE(revives_given_sums.Revives, 0) as RevivesGiven,
         COALESCE(revives_taken_sums.RevivesTaken, 0) as RevivesTaken,
         COALESCE(revives_undone_sums.KillsUndoneByRevive, 0) as KillsUndoneByRevive,
         COALESCE(revives_undone_sums.KillsUndoneByRevivePoints, 0) as KillsUndoneByRevivePoints,
         COALESCE(revives_taken_sums.PostReviveKills, 0) as PostReviveKills,
         COALESCE(revives_taken_sums.ReviveInstantDeaths, 0) as ReviveInstantDeaths,
         COALESCE(revives_taken_sums.ReviveLivesMoreThan15s, 0) as ReviveLivesMoreThan15s,
         COALESCE(revives_taken_sums.ShortestRevivedLifeSeconds, 0) as ShortestRevivedLifeSeconds,
         COALESCE(revives_taken_sums.LongestRevivedLifeSeconds, 0) as LongestRevivedLifeSeconds,
         COALESCE(revives_taken_sums.AvgRevivedLifeSeconds, 0) as AvgRevivedLifeSeconds
    FROM [dbo].ScrimMatchParticipatingPlayer match_players
      LEFT OUTER JOIN (SELECT match_players.ScrimMatchId,
                                MAX(match_players.TeamOrdinal) TeamOrdinal,
                                match_players.CharacterId CharacterId,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType IN (0, 1, 2) THEN kills.Points ELSE 0 END) Points,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType IN (0, 1, 2) THEN kills.Points
                                          WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 THEN kills.Points * -1 ELSE 0 END) NetScore,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 THEN 1 ELSE 0 END) Kills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 1 THEN 1 ELSE 0 END) TeamKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END) HeadshotKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.Points > 0 THEN 1 ELSE 0 END) ScoredKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.Points = 0 THEN 1 ELSE 0 END) ZeroPointKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN (1, 8, 15) THEN 1 ELSE 0 END)  KillsAsInfiltrator,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN (3, 10, 17) THEN 1 ELSE 0 END) KillsAsLightAssault,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN (4, 11, 18) THEN 1 ELSE 0 END) KillsAsMedic,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN (5, 12, 19) THEN 1 ELSE 0 END) KillsAsEngineer,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN (6, 13, 20) THEN 1 ELSE 0 END) KillsAsHeavyAssault,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN (7, 14, 21) THEN 1 ELSE 0 END) KillsAsMax,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND (damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0) THEN 1 ELSE 0 END) AssistedKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END) DamageAssistedKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) DamageAssistedOnlyKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END) GrenadeAssistedKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) GrenadeAssistedOnlyKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END) SpotAssistedKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END) SpotAssistedOnlyKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND (damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL) THEN 1 ELSE 0 END) UnassistedKills,
                                SUM(CASE WHEN CharacterId = kills.AttackerCharacterId AND damage_sums.TotalDamages > 0
                                             THEN CASE WHEN kills.VictimLoadoutId IN (1, 8, 15) THEN 900 - damage_sums.MinVictimDamageReceived
                                                       ELSE 1000 - damage_sums.MinVictimDamageReceived END
                                         WHEN CharacterId = kills.AttackerCharacterId AND (damage_sums.TotalDamages = 0 OR damage_sums.TotalDamages IS NULL)
                                           THEN CASE WHEN kills.VictimLoadoutId IN (1, 8, 15) THEN 900
                                                     ELSE 1000 END
                                         ELSE 0 END) KillDamageDealt,
                            
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType IN (0, 1, 2) THEN 1 ELSE 0 END) Deaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 1 THEN 1 ELSE 0 END) TeamKillDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 2 THEN 1 ELSE 0 END) Suicides,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END) HeadshotEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END) HeadshotDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.Points > 0 THEN 1 ELSE 0 END) ScoredDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.Points = 0 THEN 1 ELSE 0 END) ZeroPointDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.VictimLoadoutId IN (1, 8, 15) THEN 1 ELSE 0 END)  DeathsAsInfiltrator,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.VictimLoadoutId IN (3, 10, 17) THEN 1 ELSE 0 END) DeathsAsLightAssault,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.VictimLoadoutId IN (4, 11, 18) THEN 1 ELSE 0 END) DeathsAsMedic,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.VictimLoadoutId IN (5, 12, 19) THEN 1 ELSE 0 END) DeathsAsEngineer,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.VictimLoadoutId IN (6, 13, 20) THEN 1 ELSE 0 END) DeathsAsHeavyAssault,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND kills.VictimLoadoutId IN (7, 14, 21) THEN 1 ELSE 0 END) DeathsAsMax,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND (damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0) THEN 1 ELSE 0 END) AssistedEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END) DamageAssistedEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) DamageAssistedOnlyEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END) GrenadeAssistedEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) GrenadeAssistedOnlyEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END) SpotAssistedEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END) SpotAssistedOnlyEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END) DamageAssistedDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) DamageAssistedOnlyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END) GrenadeAssistedDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) GrenadeAssistedOnlyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END) SpotAssistedDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END) SpotAssistedOnlyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1 ELSE 0 END) UnassistedEnemyDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1 ELSE 0 END) UnassistedDeaths,
                                SUM(CASE WHEN CharacterId = kills.VictimCharacterId
                                             THEN CASE WHEN kills.AttackerLoadoutId IN (3, 10, 17) AND kills.VictimLoadoutId IN (3, 10, 17)
                                                         THEN CASE WHEN kills.NextEventTimeDiff >= 9 AND kills.PrevEventTimeDiff > 6 THEN 1
                                                                   ELSE 0 END
                                                       WHEN kills.NextEventTimeDiff >= 6 AND kills.PrevEventTimeDiff > 3 THEN 1
                                                       ELSE 0 END
                                           ELSE 0 END) TrickleDeaths_6s
                            FROM [dbo].ScrimMatchParticipatingPlayer match_players
                              LEFT OUTER JOIN (SELECT ScrimMatchId,
                                                       Timestamp,
                                                       DeathType,
                                                       AttackerCharacterId,
                                                       AttackerLoadoutId,
                                                       VictimCharacterId,
                                                       VictimLoadoutId,
                                                       IsHeadshot,
                                                       Points,
                                                       DATEDIFF(SECOND, Timestamp, LAG(Timestamp) OVER (PARTITION BY ScrimMatchId, ScrimMatchRound ORDER BY Timestamp DESC)) NextEventTimeDiff,
                                                       DATEDIFF(SECOND, LEAD(Timestamp) OVER (PARTITION BY ScrimMatchId, ScrimMatchRound ORDER BY Timestamp DESC), Timestamp) PrevEventTimeDiff
                                                  FROM [dbo].ScrimDeath) kills
                                ON match_players.ScrimMatchId = kills.ScrimMatchId
                                  AND (match_players.CharacterId = kills.AttackerCharacterId
                                        OR match_players.CharacterId = kills.VictimCharacterId) 
                              LEFT OUTER JOIN (SELECT damages.ScrimMatchId, damages.VictimCharacterId, damages.Timestamp,
                                                       COUNT(*) TotalDamages,
                                                       SUM(CASE WHEN damages.ActionType = 304 THEN 1 ELSE 0 END) EnemyDamages,
                                                       SUM(CASE WHEN damages.ActionType = 310 THEN 1 ELSE 0 END) TeamDamages,
                                                       SUM(CASE WHEN damages.ActionType = 312 THEN 1 ELSE 0 END) SelfDamages,
                                                       AVG(CASE WHEN damages.ActionType = 304 THEN AdjustedAssistDamageDealt ELSE 0 END) AvgVictimDamageReceived,
                                                       MIN(CASE WHEN damages.ActionType = 304 THEN AdjustedAssistDamageDealt ELSE 0 END) MinVictimDamageReceived
                                                  FROM [dbo].View_ScrimDamageAssistDamageDealt damages
                                                  GROUP BY ScrimMatchId, Timestamp, VictimCharacterId) damage_sums
                                ON kills.ScrimMatchId = damage_sums.ScrimMatchId
                                  AND kills.Timestamp = damage_sums.Timestamp
                                  AND kills.VictimCharacterId = damage_sums.VictimCharacterId
                              LEFT OUTER JOIN (SELECT ScrimMatchId, VictimCharacterId, grenades.Timestamp,
                                                       COUNT(*) TotalGrenades,
                                                       SUM(CASE WHEN grenades.ActionType = 306 THEN 1 ELSE 0 END) EnemyGrenades,
                                                       SUM(CASE WHEN grenades.ActionType = 311 THEN 1 ELSE 0 END) TeamGrenades,
                                                       SUM(CASE WHEN grenades.ActionType = 313 THEN 1 ELSE 0 END) SelfGrenades
                                                  FROM [dbo].[ScrimGrenadeAssist] grenades
                                                  GROUP BY ScrimMatchId, Timestamp, VictimCharacterId) grenade_sums
                                    ON kills.ScrimMatchId = grenade_sums.ScrimMatchId
                                        AND kills.Timestamp = grenade_sums.Timestamp
                                        AND kills.VictimCharacterId = grenade_sums.VictimCharacterId
                              LEFT OUTER JOIN (SELECT ScrimMatchId, VictimCharacterId, spots.Timestamp,
                                                       COUNT(*) TotalSpots
                                                  FROM [dbo].[ScrimSpotAssist] spots
                                                  GROUP BY ScrimMatchId, SpotterTeamOrdinal, Timestamp, VictimCharacterId) spot_sums
                                    ON kills.ScrimMatchId = spot_sums.ScrimMatchId
                                        AND kills.Timestamp = spot_sums.Timestamp
                                        AND kills.VictimCharacterId = spot_sums.VictimCharacterId
                            GROUP BY match_players.ScrimMatchId, match_players.CharacterId) kill_sums
        ON match_players.ScrimMatchId = kill_sums.ScrimMatchId
            AND match_players.TeamOrdinal = kill_sums.TeamOrdinal
            AND match_players.CharacterId = kill_sums.CharacterId
      LEFT OUTER JOIN (SELECT match_players.ScrimMatchId,
                                match_players.CharacterId,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 THEN 1 ELSE 0 END) DamageAssists,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 310 THEN 1 ELSE 0 END) DamageTeamAssists,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN (1, 8, 15) THEN 1 ELSE 0 END)  DamageAssistsAsInfiltrator,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN (3, 10, 17) THEN 1 ELSE 0 END) DamageAssistsAsLightAssault,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN (4, 11, 18) THEN 1 ELSE 0 END) DamageAssistsAsMedic,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN (5, 12, 19) THEN 1 ELSE 0 END) DamageAssistsAsEngineer,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN (6, 13, 20) THEN 1 ELSE 0 END) DamageAssistsAsHeavyAssault,
                                SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN (7, 14, 21) THEN 1 ELSE 0 END) DamageAssistsAsMax,
                                SUM(CASE WHEN CharacterID = damages.AttackerCharacterId AND damages.ActionType = 304 THEN AdjustedAssistDamageDealt ELSE 0 END) AssistDamageDealt
                          FROM [dbo].ScrimMatchParticipatingPlayer match_players
                            INNER JOIN [dbo].View_ScrimDamageAssistDamageDealt damages
                              ON match_players.ScrimMatchId = damages.ScrimMatchId
                                  AND  match_players.CharacterId = damages.AttackerCharacterId
                          GROUP BY match_players.ScrimMatchId, match_players.CharacterId) damage_sums
                ON match_players.ScrimMatchId = damage_sums.ScrimMatchId 
                  AND match_players.CharacterId = damage_sums.CharacterId
      LEFT OUTER JOIN (SELECT ScrimMatchId,
                              MedicCharacterId,
                              COUNT(1) Revives,
                              SUM(Points) Points
                        FROM [dbo].ScrimRevive
                        GROUP BY ScrimMatchId, MedicCharacterId) revives_given_sums
        ON match_players.ScrimMatchId = revives_given_sums.ScrimMatchId
          AND match_players.CharacterId = revives_given_sums.MedicCharacterId
      LEFT OUTER JOIN (SELECT ScrimMatchId
                             ,RevivedCharacterId
                             ,COUNT(1) as RevivesTaken
                             ,SUM(EnemyPoints) as EnemyPoints
                             ,SUM(KillCount) as PostReviveKills
                             ,SUM(IIF(RevivedLifeSeconds <= 3, 1, 0)) as ReviveInstantDeaths
                             ,SUM(IIF(RevivedLifeSeconds <= 1, 1, 0)) as ReviveLivesLessThanOrEqual1s
                             ,SUM(IIF(RevivedLifeSeconds <= 5, 1, 0)) as ReviveLivesLessThanOrEqual5s
                             ,SUM(IIF(RevivedLifeSeconds <= 10, 1, 0)) as ReviveLivesLessThanOrEqual10s
                             ,SUM(IIF(RevivedLifeSeconds < 15, 1, 0)) as ReviveLivesLessThan15s
                             ,SUM(IIF(RevivedLifeSeconds >= 15, 1, 0)) as ReviveLivesMoreThan15s
                             ,MIN(RevivedLifeSeconds) as ShortestRevivedLifeSeconds
                             ,MAX(RevivedLifeSeconds) as LongestRevivedLifeSeconds
                             ,AVG(RevivedLifeSeconds) as AvgRevivedLifeSeconds
                         FROM (SELECT DISTINCT
                                      revive_idx.ScrimMatchId
                                     ,revive_idx.ScrimMatchRound
                                     ,revive_idx.Timestamp as ReviveTime
                                     ,revive_idx.RevivedCharacterId
                                     ,revive_idx.RevivedTeamOrdinal as Team
                                     ,revive_idx.NextReviveTimestamp
                                     ,revive_idx.EnemyPoints
                                     ,death.Timestamp as DeathTime
                                     ,CASE WHEN death.Timestamp IS NULL THEN NULL
                                           ELSE CONVERT(int, DATEDIFF(ms, revive_idx.Timestamp, death.Timestamp) / 1000.0) END as RevivedLifeSeconds
                                     ,COUNT(kills.Timestamp) OVER (PARTITION BY revive_idx.ScrimMatchId, revive_idx.ScrimMatchRound, revive_idx.RevivedCharacterId, revive_idx.Timestamp, death.Timestamp) as KillCount
                                     ,ROW_NUMBER() OVER (PARTITION BY revive_idx.ScrimMatchId, revive_idx.ScrimMatchRound, revive_idx.RevivedCharacterId, revive_idx.Timestamp ORDER BY death.Timestamp ASC) as RowNum
                                 FROM (SELECT ScrimMatchId
                                             ,RevivedTeamOrdinal
                                             ,Timestamp
                                             ,RevivedCharacterId
                                             ,ScrimMatchRound
                                             ,EnemyPoints
                                             ,LEAD(Timestamp) OVER (PARTITION BY ScrimMatchId, RevivedCharacterId, ScrimMatchRound ORDER BY Timestamp ASC) as NextReviveTimestamp
                                         FROM ScrimRevive revive) revive_idx
                                           LEFT OUTER JOIN ScrimDeath death
                                             ON revive_idx.ScrimMatchId = death.ScrimMatchId
                                                 AND revive_idx.ScrimMatchRound = death.ScrimMatchRound
                                                 AND revive_idx.RevivedCharacterId = death.VictimCharacterId
                                                 AND death.Timestamp > revive_idx.Timestamp
                                                 AND death.Timestamp < revive_idx.NextReviveTimestamp
                                           LEFT OUTER JOIN ScrimDeath kills
                                             ON revive_idx.ScrimMatchId = kills.ScrimMatchId
                                                 AND revive_idx.ScrimMatchRound = kills.ScrimMatchRound
                                                 AND revive_idx.RevivedCharacterId = kills.AttackerCharacterId
                                                 AND kills.Timestamp > revive_idx.Timestamp
                                                 AND kills.Timestamp < revive_idx.NextReviveTimestamp
                                                 AND kills.Timestamp < death.Timestamp) revive_taken_times
                                 WHERE revive_taken_times.RowNum = 1
                                 GROUP BY ScrimMatchId, RevivedCharacterId) revives_taken_sums
      --LEFT OUTER JOIN (SELECT ScrimMatchId,
      --                        RevivedCharacterId,
      --                        COUNT(1) Revives,
      --                        SUM(EnemyPoints) as EnemyPoints
      --                    FROM [dbo].ScrimRevive
      --                    GROUP BY ScrimMatchId, RevivedCharacterId) revives_taken_sums
        ON match_players.ScrimMatchId = revives_taken_sums.ScrimMatchId
          AND match_players.CharacterId = revives_taken_sums.RevivedCharacterId
      LEFT OUTER JOIN (SELECT ScrimMatchId,
                              LastKilledByCharacterId,
                              COUNT(1) as KillsUndoneByRevive,
                              SUM(EnemyPoints) as KillsUndoneByRevivePoints
                           FROM [dbo].ScrimRevive revive
                          GROUP BY ScrimMatchId, LastKilledByCharacterId) revives_undone_sums
        ON match_players.ScrimMatchId = revives_undone_sums.ScrimMatchId
          AND match_players.CharacterId = revives_undone_sums.LastKilledByCharacterId