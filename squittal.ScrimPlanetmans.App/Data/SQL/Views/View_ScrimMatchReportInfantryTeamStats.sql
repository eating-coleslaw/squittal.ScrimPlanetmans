USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchReportInfantryTeamStats'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchReportInfantryTeamStats as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchReportInfantryTeamStats AS
-- CREATE OR ALTER VIEW View_ScrimMatchReportInfantryTeamStats AS

  SELECT match_teams.ScrimMatchId,
         match_teams.TeamOrdinal,
         MAX(COALESCE(kill_sums.Points, 0)
             + COALESCE(capture_sums.Points, 0)
             + COALESCE(revive_sums.Points, 0)
             + COALESCE(enemy_revive_sums.EnemyPoints, 0)
             + COALESCE(periodic_tick_sums.PeriodicControlTickPoints, 0)) Points,
         MAX(COALESCE(kill_sums.NetScore, 0)
             + COALESCE(capture_sums.Points, 0)
             + COALESCE(revive_sums.Points, 0)
             - COALESCE(revive_sums.EnemyPoints, 0)
             + COALESCE(enemy_revive_sums.EnemyPoints, 0)
             + COALESCE(periodic_tick_sums.PeriodicControlTickPoints, 0)) NetScore,
         MAX(COALESCE(adjustment_sums.Points, 0)) PointAdjustments,
         MAX(COALESCE(capture_sums.Points, 0)) FacilityCapturePoints,
         MAX(COALESCE(kill_sums.Kills, 0)) Kills,
         MAX(COALESCE(kill_sums.HeadshotKills, 0)) HeadshotKills,
         MAX(COALESCE(kill_sums.Deaths, 0)) Deaths,
         MAX(COALESCE(kill_sums.HeadshotEnemyDeaths, 0)) HeadshotEnemyDeaths,
         MAX(COALESCE(kill_sums.TeamKills, 0)) TeamKills,
         MAX(COALESCE(kill_sums.Suicides, 0)) Suicides,
         MAX(COALESCE(kill_sums.ScoredDeaths, 0)) ScoredDeaths,
         MAX(COALESCE(kill_sums.ZeroPointDeaths, 0)) ZeroPointDeaths,
         MAX(COALESCE(kill_sums.TeamKillDeaths, 0)) TeamKillDeaths,
         MAX(COALESCE(kill_sums.TrickleDeaths_6s, 0)) TrickleDeaths,
         MAX(COALESCE(damage_sums.DamageAssists, 0)) DamageAssists,
         MAX(COALESCE(grenade_sums.GrenadeAssistedKills, 0)) GrenadeAssists,
         MAX(COALESCE(spot_sums.SpotAssistedKills, 0)) SpotAssists,
         MAX(COALESCE(damage_sums.DamageTeamAssists, 0)) DamageTeamAssists,
         MAX(COALESCE(grenade_sums.GrenadeTeamAssistedKills, 0)) GrenadeTeamAssists,
         MAX(COALESCE(kill_sums.DamageAssistedKills, 0)) DamageAssistedKills,
         MAX(COALESCE(kill_sums.DamageAssistedOnlyKills, 0)) DamageAssistedOnlyKills,
         MAX(COALESCE(kill_sums.GrenadeAssistedKills, 0)) GrenadeAssistedKills,
         MAX(COALESCE(kill_sums.GrenadeAssistedOnlyKills, 0)) GrenadeAssistedOnlyKills,
         MAX(COALESCE(kill_sums.SpotAssistedKills, 0)) SpotAssistedKills,
         MAX(COALESCE(kill_sums.SpotAssistedOnlyKills, 0)) SpotAssistedOnlyKills,
         MAX(COALESCE(kill_sums.AssistedKills, 0)) AssistedKills,
         MAX(COALESCE(kill_sums.DamageAssistedDeaths, 0)) DamageAssistedDeaths,
         MAX(COALESCE(kill_sums.DamageAssistedOnlyDeaths, 0)) DamageAssistedOnlyDeaths,
         MAX(COALESCE(kill_sums.GrenadeAssistedDeaths, 0)) GrenadeAssistedDeaths,
         MAX(COALESCE(kill_sums.GrenadeAssistedOnlyDeaths, 0)) GrenadeAssistedOnlyDeaths,
         MAX(COALESCE(kill_sums.SpotAssistedDeaths, 0)) SpotAssistedDeaths,
         MAX(COALESCE(kill_sums.SpotAssistedOnlyDeaths, 0)) SpotAssistedOnlyDeaths,
         MAX(COALESCE(kill_sums.DamageAssistedEnemyDeaths, 0)) DamageAssistedEnemyDeaths,
         MAX(COALESCE(kill_sums.DamageAssistedOnlyEnemyDeaths, 0)) DamageAssistedOnlyEnemyDeaths,
         MAX(COALESCE(kill_sums.GrenadeAssistedEnemyDeaths, 0)) GrenadeAssistedEnemyDeaths,
         MAX(COALESCE(kill_sums.GrenadeAssistedOnlyEnemyDeaths, 0)) GrenadeAssistedOnlyEnemyDeaths,
         MAX(COALESCE(kill_sums.SpotAssistedEnemyDeaths, 0)) SpotAssistedEnemyDeaths,
         MAX(COALESCE(kill_sums.SpotAssistedOnlyEnemyDeaths, 0)) SpotAssistedOnlyEnemyDeaths,
         MAX(COALESCE(kill_sums.UnassistedEnemyDeaths, 0)) UnassistedEnemyDeaths,
         MAX(COALESCE(kill_sums.KillsAsHeavyAssault, 0)) KillsAsHeavyAssault,
         MAX(COALESCE(kill_sums.KillsAsInfiltrator, 0)) KillsAsInfiltrator,
         MAX(COALESCE(kill_sums.KillsAsLightAssault, 0)) KillsAsLightAssault,
         MAX(COALESCE(kill_sums.KillsAsMedic, 0)) KillsAsMedic,
         MAX(COALESCE(kill_sums.KillsAsEngineer, 0)) KillsAsEngineer,
         MAX(COALESCE(kill_sums.KillsAsMax, 0)) KillsAsMax,
         MAX(COALESCE(kill_sums.DeathsAsHeavyAssault, 0)) DeathsAsHeavyAssault,
         MAX(COALESCE(kill_sums.DeathsAsInfiltrator, 0)) DeathsAsInfiltrator,
         MAX(COALESCE(kill_sums.DeathsAsLightAssault, 0)) DeathsAsLightAssault,
         MAX(COALESCE(kill_sums.DeathsAsMedic, 0)) DeathsAsMedic,
         MAX(COALESCE(kill_sums.DeathsAsEngineer, 0)) DeathsAsEngineer,
         MAX(COALESCE(kill_sums.DeathsAsMax, 0)) DeathsAsMax,
         MAX(COALESCE(damage_sums.DamageAssistsAsHeavyAssault, 0)) DamageAssistsAsHeavyAssault,
         MAX(COALESCE(damage_sums.DamageAssistsAsInfiltrator, 0)) DamageAssistsAsInfiltrator,
         MAX(COALESCE(damage_sums.DamageAssistsAsLightAssault, 0)) DamageAssistsAsLightAssault,
         MAX(COALESCE(damage_sums.DamageAssistsAsMedic, 0)) DamageAssistsAsMedic,
         MAX(COALESCE(damage_sums.DamageAssistsAsEngineer, 0)) DamageAssistsAsEngineer,
         MAX(COALESCE(damage_sums.DamageAssistsAsMax, 0)) DamageAssistsAsMax,
         CAST(MAX(ROUND(COALESCE(kill_sums.KillDamageDealt, 0), 0)) AS int) KillDamageDealt,
         CAST(MAX(ROUND(COALESCE(damage_sums.AssistDamageDealt, 0), 0)) AS int) AssistDamageDealt,
         CAST(MAX(ROUND(COALESCE(kill_sums.KillDamageDealt, 0) + COALESCE(damage_sums.AssistDamageDealt, 0), 0)) AS int) TotalDamageDealt,
         MAX(COALESCE(revive_sums.Revives, 0)) as Revives,
         MAX(COALESCE(enemy_revive_sums.Revives, 0)) as EnemyRevivesAllowed,
         MAX(COALESCE(periodic_tick_sums.PeriodicControlTickCount, 0)) as PeriodicControlTicks,
         MAX(COALESCE(periodic_tick_sums.PeriodicControlTickPoints, 0)) as PeriodicControlTickPoints,
         MAX(COALESCE(revives_taken_sums.PostReviveKills, 0)) as PostReviveKills,
         MAX(COALESCE(revives_taken_sums.ReviveInstantDeaths, 0)) as ReviveInstantDeaths,
         MAX(COALESCE(revives_taken_sums.ReviveLivesMoreThan15s, 0)) as ReviveLivesMoreThan15s,
         MAX(COALESCE(revives_taken_sums.ShortestRevivedLifeSeconds, 0)) as ShortestRevivedLifeSeconds,
         MAX(COALESCE(revives_taken_sums.LongestRevivedLifeSeconds, 0)) as LongestRevivedLifeSeconds,
         MAX(COALESCE(revives_taken_sums.AvgRevivedLifeSeconds, 0)) as AvgRevivedLifeSeconds
    FROM (SELECT match_players.ScrimMatchId,
                  match_players.TeamOrdinal
             FROM [dbo].ScrimMatchParticipatingPlayer match_players
             GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal) match_teams
      LEFT OUTER JOIN (SELECT match_teams.ScrimMatchId,
                               match_teams.TeamOrdinal TeamOrdinal,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal THEN kills.Points ELSE 0 END) Points,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal THEN kills.Points
                                         WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 THEN kills.Points * -1 ELSE 0 END) NetScore, 
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 THEN 1 ELSE 0 END) Kills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 1 THEN 1 ELSE 0 END) TeamKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END) HeadshotKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.Points > 0 THEN 1 ELSE 0 END) ScoredKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.Points = 0 THEN 1 ELSE 0 END) ZeroPointKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.AttackerLoadoutId IN (1, 8, 15) THEN 1 ELSE 0 END)  KillsAsInfiltrator,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.AttackerLoadoutId IN (3, 10, 17) THEN 1 ELSE 0 END) KillsAsLightAssault,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.AttackerLoadoutId IN (4, 11, 18) THEN 1 ELSE 0 END) KillsAsMedic,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.AttackerLoadoutId IN (5, 12, 19) THEN 1 ELSE 0 END) KillsAsEngineer,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.AttackerLoadoutId IN (6, 13, 20) THEN 1 ELSE 0 END) KillsAsHeavyAssault,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.AttackerLoadoutId IN (7, 14, 21) THEN 1 ELSE 0 END) KillsAsMax,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND (damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0) THEN 1 ELSE 0 END) AssistedKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END) DamageAssistedKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) DamageAssistedOnlyKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END) GrenadeAssistedKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) GrenadeAssistedOnlyKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END) SpotAssistedKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END) SpotAssistedOnlyKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND (damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL) THEN 1 ELSE 0 END) UnassistedKills,
                               SUM(CASE WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND damage_sums.TotalDamages > 0
                                             THEN CASE WHEN kills.VictimLoadoutId IN (1, 8, 15) THEN 900 - damage_sums.MinVictimDamageReceived
                                                       ELSE 1000 - damage_sums.MinVictimDamageReceived END
                                         WHEN kills.AttackerTeamOrdinal = match_teams.TeamOrdinal AND (damage_sums.TotalDamages = 0 OR damage_sums.TotalDamages IS NULL)
                                           THEN CASE WHEN kills.VictimLoadoutId IN (1, 8, 15) THEN 900
                                                     ELSE 1000 END
                                         ELSE 0 END) KillDamageDealt,
                           
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType IN (0, 1, 2) THEN 1 ELSE 0 END) Deaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 1 THEN 1 ELSE 0 END) TeamKillDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 2 THEN 1 ELSE 0 END) Suicides,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END) HeadshotEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END) HeadshotDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.Points > 0 THEN 1 ELSE 0 END) ScoredDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.Points = 0 THEN 1 ELSE 0 END) ZeroPointDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.VictimLoadoutId IN (1, 8, 15) THEN 1 ELSE 0 END)  DeathsAsInfiltrator,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.VictimLoadoutId IN (3, 10, 17) THEN 1 ELSE 0 END) DeathsAsLightAssault,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.VictimLoadoutId IN (4, 11, 18) THEN 1 ELSE 0 END) DeathsAsMedic,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.VictimLoadoutId IN (5, 12, 19) THEN 1 ELSE 0 END) DeathsAsEngineer,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.VictimLoadoutId IN (6, 13, 20) THEN 1 ELSE 0 END) DeathsAsHeavyAssault,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND kills.VictimLoadoutId IN (7, 14, 21) THEN 1 ELSE 0 END) DeathsAsMax,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND (damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0) THEN 1 ELSE 0 END) AssistedEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END) DamageAssistedEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) DamageAssistedOnlyEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END) GrenadeAssistedEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) GrenadeAssistedOnlyEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END) SpotAssistedEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END) SpotAssistedOnlyEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END) DamageAssistedDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) DamageAssistedOnlyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END) GrenadeAssistedDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END) GrenadeAssistedOnlyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END) SpotAssistedDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END) SpotAssistedOnlyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1 ELSE 0 END) UnassistedEnemyDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1 ELSE 0 END) UnassistedDeaths,
                               SUM(CASE WHEN kills.VictimTeamOrdinal = match_teams.TeamOrdinal
                                             THEN CASE WHEN kills.AttackerLoadoutId IN (3, 10, 17) AND kills.VictimLoadoutId IN (3, 10, 17)
                                                         THEN CASE WHEN kills.NextEventTimeDiff >= 9 AND kills.PrevEventTimeDiff > 6 THEN 1
                                                                   ELSE 0 END
                                                       WHEN kills.NextEventTimeDiff >= 6 AND kills.PrevEventTimeDiff > 3 THEN 1
                                                       ELSE 0 END
                                           ELSE 0 END) TrickleDeaths_6s
                               
                          FROM (SELECT match_teams.ScrimMatchId,
                                        match_teams.TeamOrdinal
                                   FROM [dbo].ScrimMatchParticipatingPlayer match_teams
                                   GROUP BY match_teams.ScrimMatchId, match_teams.TeamOrdinal) match_teams
                            LEFT OUTER JOIN (SELECT ScrimMatchId,
                                                       Timestamp,
                                                       DeathType,
                                                       AttackerTeamOrdinal,
                                                       AttackerCharacterId,
                                                       AttackerLoadoutId,
                                                       VictimTeamOrdinal,
                                                       VictimCharacterId,
                                                       VictimLoadoutId,
                                                       IsHeadshot,
                                                       Points,
                                                       DATEDIFF(SECOND, Timestamp, LAG(Timestamp) OVER (PARTITION BY ScrimMatchId, ScrimMatchRound ORDER BY Timestamp DESC)) NextEventTimeDiff,
                                                       DATEDIFF(SECOND, LEAD(Timestamp) OVER (PARTITION BY ScrimMatchId, ScrimMatchRound ORDER BY Timestamp DESC), Timestamp) PrevEventTimeDiff
                                                  FROM [dbo].ScrimDeath) kills
                              ON match_teams.ScrimMatchId = kills.ScrimMatchId
                                 AND (match_teams.TeamOrdinal = kills.AttackerTeamOrdinal
                                       OR match_teams.TeamOrdinal = kills.VictimTeamOrdinal)
                            LEFT OUTER JOIN (SELECT ScrimMatchId, damages.AttackerTeamOrdinal, VictimCharacterId, damages.Timestamp,
                                                     COUNT(*) TotalDamages,
                                                     SUM(CASE WHEN damages.ActionType = 304 THEN 1 ELSE 0 END) EnemyDamages,
                                                     SUM(CASE WHEN damages.ActionType = 310 THEN 1 ELSE 0 END) TeamDamages,
                                                     SUM(CASE WHEN damages.ActionType = 312 THEN 1 ELSE 0 END) SelfDamages,
                                                     AVG(CASE WHEN damages.ActionType = 304 THEN AdjustedAssistDamageDealt ELSE 0 END) AvgVictimDamageReceived,
                                                     MIN(CASE WHEN damages.ActionType = 304 THEN AdjustedAssistDamageDealt ELSE 0 END) MinVictimDamageReceived
                                                FROM [dbo].View_ScrimDamageAssistDamageDealt damages
                                                GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId) damage_sums
                              ON kills.ScrimMatchId = damage_sums.ScrimMatchId
                                 AND kills.Timestamp = damage_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = damage_sums.AttackerTeamOrdinal
                                 AND kills.VictimCharacterId = damage_sums.VictimCharacterId
                            LEFT OUTER JOIN (SELECT ScrimMatchId, grenades.AttackerTeamOrdinal, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades,
                                                     SUM(CASE WHEN grenades.ActionType = 306 THEN 1 ELSE 0 END) EnemyGrenades,
                                                     SUM(CASE WHEN grenades.ActionType = 311 THEN 1 ELSE 0 END) TeamGrenades,
                                                     SUM(CASE WHEN grenades.ActionType = 313 THEN 1 ELSE 0 END) SeldGrenades
                                                FROM [dbo].[ScrimGrenadeAssist] grenades
                                                GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId) grenade_sums
                              ON kills.ScrimMatchId = grenade_sums.ScrimMatchId
                                 AND kills.Timestamp = grenade_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = grenade_sums.AttackerTeamOrdinal
                                 AND kills.VictimCharacterId = grenade_sums.VictimCharacterId
                            LEFT OUTER JOIN (SELECT ScrimMatchId, spots.SpotterTeamOrdinal, VictimCharacterId, spots.Timestamp, COUNT(*) TotalSpots
                                                FROM [dbo].[ScrimSpotAssist] spots
                                                GROUP BY ScrimMatchId, SpotterTeamOrdinal, Timestamp, VictimCharacterId) spot_sums
                              ON kills.ScrimMatchId = spot_sums.ScrimMatchId
                                 AND kills.Timestamp = spot_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = spot_sums.SpotterTeamOrdinal
                                 AND kills.VictimCharacterId = spot_sums.VictimCharacterId
                          GROUP BY match_teams.ScrimMatchId, match_teams.TeamOrdinal) kill_sums
        ON match_teams.ScrimMatchId = kill_sums.ScrimMatchId
           AND match_teams.TeamOrdinal = kill_sums.TeamOrdinal
      LEFT OUTER JOIN (SELECT match_players.ScrimMatchId,
                               match_players.TeamOrdinal,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 THEN 1 ELSE 0 END) DamageAssists,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 310 THEN 1 ELSE 0 END) DamageTeamAssists,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN (1, 8, 15) THEN 1 ELSE 0 END)  DamageAssistsAsInfiltrator,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN (3, 10, 17) THEN 1 ELSE 0 END) DamageAssistsAsLightAssault,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN (4, 11, 18) THEN 1 ELSE 0 END) DamageAssistsAsMedic,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN (5, 12, 19) THEN 1 ELSE 0 END) DamageAssistsAsEngineer,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN (6, 13, 20) THEN 1 ELSE 0 END) DamageAssistsAsHeavyAssault,
                               SUM(CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN (7, 14, 21) THEN 1 ELSE 0 END) DamageAssistsAsMax,
                               SUM(CASE WHEN CharacterID = damages.AttackerCharacterId AND damages.ActionType = 304 THEN damages.AdjustedAssistDamageDealt ELSE 0 END) AssistDamageDealt
                          FROM [dbo].ScrimMatchParticipatingPlayer match_players
                            INNER JOIN [dbo].View_ScrimDamageAssistDamageDealt damages
                              ON match_players.ScrimMatchId = damages.ScrimMatchId
                                 AND  match_players.CharacterId = damages.AttackerCharacterId
                          GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal) damage_sums
        ON match_teams.ScrimMatchId = damage_sums.ScrimMatchId 
          AND match_teams.TeamOrdinal = damage_sums.TeamOrdinal
      LEFT OUTER JOIN (SELECT match_players.ScrimMatchId,
                               match_players.TeamOrdinal,
                               SUM(CASE WHEN CharacterId = grenades.AttackerCharacterId AND grenades.ActionType = 306 THEN 1 ELSE 0 END) GrenadeAssistedKills,
                               SUM(CASE WHEN CharacterId = grenades.AttackerCharacterId AND grenades.ActionType = 311 THEN 1 ELSE 0 END) GrenadeTeamAssistedKills
                          FROM [dbo].ScrimMatchParticipatingPlayer match_players
                            INNER JOIN [dbo].ScrimGrenadeAssist grenades
                              ON match_players.ScrimMatchId = grenades.ScrimMatchId
                                 AND  match_players.CharacterId = grenades.AttackerCharacterId
                          GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal) grenade_sums
        ON match_teams.ScrimMatchId = grenade_sums.ScrimMatchId
          AND match_teams.TeamOrdinal = grenade_sums.TeamOrdinal
      LEFT OUTER JOIN (SELECT match_players.ScrimMatchId,
                               match_players.TeamOrdinal,
                               SUM(CASE WHEN CharacterId = spots.SpotterCharacterId AND spots.ActionType = 308 THEN 1 ELSE 0 END) SpotAssistedKills
                          FROM [dbo].ScrimMatchParticipatingPlayer match_players
                            INNER JOIN [dbo].ScrimSpotAssist spots
                              ON match_players.ScrimMatchId = spots.ScrimMatchId
                                 AND  match_players.CharacterId = spots.SpotterCharacterId
                          GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal) spot_sums
        ON match_teams.ScrimMatchId = spot_sums.ScrimMatchId 
          AND match_teams.TeamOrdinal = spot_sums.TeamOrdinal
      LEFT OUTER JOIN (SELECT adjustments.ScrimMatchId,
                               adjustments.TeamOrdinal,
                               SUM(Points) Points
                          FROM [dbo].ScrimMatchTeamPointAdjustment adjustments
                          GROUP BY adjustments.ScrimMatchId, adjustments.TeamOrdinal) adjustment_sums
        ON adjustment_sums.ScrimMatchId = match_teams.ScrimMatchId
           AND adjustment_sums.TeamOrdinal = match_teams.TeamOrdinal
      LEFT OUTER JOIN (SELECT captures.ScrimMatchId,
                               captures.ControllingTeamOrdinal TeamOrdinal,
                               SUM(captures.Points) Points
                          FROM [dbo].ScrimFacilityControl captures
                          GROUP BY captures.ScrimMatchId, captures.ControllingTeamOrdinal) capture_sums
        ON match_teams.ScrimMatchId = capture_sums.ScrimMatchId
           AND match_teams.TeamOrdinal = capture_sums.TeamOrdinal
        LEFT OUTER JOIN (SELECT ScrimMatchId,
                                 MedicTeamOrdinal TeamOrdinal,
                                 COUNT(1) Revives,
                                 SUM(Points) Points,
                                 SUM(EnemyPoints) EnemyPoints
                            FROM [dbo].ScrimRevive
                            GROUP BY ScrimMatchId, MedicTeamOrdinal) revive_sums
          ON match_teams.ScrimMatchId = revive_sums.ScrimMatchId
            AND match_teams.TeamOrdinal = revive_sums.TeamOrdinal
        LEFT OUTER JOIN (SELECT ScrimMatchId,
                                 MedicTeamOrdinal TeamOrdinal,
                                 COUNT(1) Revives,
                                 SUM(EnemyPoints) EnemyPoints
                            FROM [dbo].ScrimRevive
                            GROUP BY ScrimMatchId, MedicTeamOrdinal) enemy_revive_sums
          ON match_teams.ScrimMatchId = enemy_revive_sums.ScrimMatchId
            AND match_teams.TeamOrdinal <> enemy_revive_sums.TeamOrdinal
      LEFT OUTER JOIN (SELECT ScrimMatchId
                             ,TeamOrdinal
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
                                     ,revive_idx.RevivedTeamOrdinal as TeamOrdinal
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
                                 GROUP BY ScrimMatchId, TeamOrdinal) revives_taken_sums
          ON match_teams.ScrimMatchId = revives_taken_sums.ScrimMatchId
            AND match_teams.TeamOrdinal = revives_taken_sums.TeamOrdinal
        LEFT OUTER JOIN (SELECT ScrimMatchId,
                                TeamOrdinal,
                                COUNT(1) as PeriodicControlTickCount,
                                SUM(Points) as PeriodicControlTickPoints
                           FROM [dbo].ScrimPeriodicControlTick
                           GROUP BY ScrimMatchId, TeamOrdinal) periodic_tick_sums
          ON match_teams.ScrimMatchId = periodic_tick_sums.ScrimMatchId
            AND match_teams.TeamOrdinal = periodic_tick_sums.TeamOrdinal
  GROUP BY match_teams.ScrimMatchId, match_teams.TeamOrdinal