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
         MAX( ( COALESCE(kill_sums.Points, 0) +  COALESCE(adjustment_sums.Points, 0) +  COALESCE(capture_sums.Points, 0) ) ) Points,
         MAX( ( COALESCE(kill_sums.NetScore, 0) + COALESCE(death_sums.NetScore, 0) + COALESCE(adjustment_sums.Points, 0) + COALESCE(capture_sums.Points, 0) ) ) NetScore,
         MAX( COALESCE(adjustment_sums.Points, 0) ) PointAdjustments,
         MAX( COALESCE(capture_sums.Points, 0) ) FacilityCapturePoints,
         MAX( COALESCE(kill_sums.Kills, 0) ) Kills,
         MAX( COALESCE(kill_sums.HeadshotKills, 0) ) HeadshotKills,
         MAX( COALESCE(death_sums.Deaths, 0) ) Deaths,
         MAX( COALESCE(death_sums.HeadshotEnemyDeaths, 0) ) HeadshotEnemyDeaths,
         MAX( COALESCE(kill_sums.TeamKills, 0) ) TeamKills,
         MAX( COALESCE(death_sums.Suicides, 0) ) Suicides,
         MAX( COALESCE(death_sums.ScoredDeaths, 0) ) ScoredDeaths,
         MAX( COALESCE(death_sums.ZeroPointDeaths, 0) ) ZeroPointDeaths,
         MAX( COALESCE(death_sums.TeamKillDeaths, 0) ) TeamKillDeaths,
         MAX( COALESCE(damage_sums.DamageAssists, 0) ) DamageAssists,
         MAX( COALESCE(grenade_sums.GrenadeAssistedKills, 0) ) GrenadeAssists,
         MAX( COALESCE(spot_sums.SpotAssistedKills, 0) ) SpotAssists,
         MAX( COALESCE(damage_sums.DamageTeamAssists, 0) ) DamageTeamAssists,
         MAX( COALESCE(grenade_sums.GrenadeTeamAssistedKills, 0) ) GrenadeTeamAssists,
         MAX( COALESCE(kill_sums.DamageAssistedKills, 0) ) DamageAssistedKills,
         MAX( COALESCE(kill_sums.DamageAssistedOnlyKills, 0) ) DamageAssistedOnlyKills,
         MAX( COALESCE(kill_sums.GrenadeAssistedKills, 0) ) GrenadeAssistedKills,
         MAX( COALESCE(kill_sums.GrenadeAssistedOnlyKills, 0) ) GrenadeAssistedOnlyKills,
         MAX( COALESCE(kill_sums.SpotAssistedKills, 0) ) SpotAssistedKills,
         MAX( COALESCE(kill_sums.SpotAssistedOnlyKills, 0) ) SpotAssistedOnlyKills,
         MAX( COALESCE(kill_sums.AssistedKills, 0) ) AssistedKills,
         MAX( COALESCE(death_sums.DamageAssistedDeaths, 0) ) DamageAssistedDeaths,
         MAX( COALESCE(death_sums.DamageAssistedOnlyDeaths, 0) ) DamageAssistedOnlyDeaths,
         MAX( COALESCE(death_sums.GrenadeAssistedDeaths, 0) ) GrenadeAssistedDeaths,
         MAX( COALESCE(death_sums.GrenadeAssistedOnlyDeaths, 0) ) GrenadeAssistedOnlyDeaths,
         MAX( COALESCE(death_sums.SpotAssistedDeaths, 0) ) SpotAssistedDeaths,
         MAX( COALESCE(death_sums.SpotAssistedOnlyDeaths, 0) ) SpotAssistedOnlyDeaths,
         MAX( COALESCE(death_sums.DamageAssistedEnemyDeaths, 0) ) DamageAssistedEnemyDeaths,
         MAX( COALESCE(death_sums.DamageAssistedOnlyEnemyDeaths, 0) ) DamageAssistedOnlyEnemyDeaths,
         MAX( COALESCE(death_sums.GrenadeAssistedEnemyDeaths, 0) ) GrenadeAssistedEnemyDeaths,
         MAX( COALESCE(death_sums.GrenadeAssistedOnlyEnemyDeaths, 0) ) GrenadeAssistedOnlyEnemyDeaths,
         MAX( COALESCE(death_sums.SpotAssistedEnemyDeaths, 0) ) SpotAssistedEnemyDeaths,
         MAX( COALESCE(death_sums.SpotAssistedOnlyEnemyDeaths, 0) ) SpotAssistedOnlyEnemyDeaths,
         MAX( COALESCE(death_sums.UnassistedEnemyDeaths, 0) ) UnassistedEnemyDeaths,
         MAX( COALESCE(kill_sums.KillsAsHeavyAssault, 0) ) KillsAsHeavyAssault,
         MAX( COALESCE(kill_sums.KillsAsInfiltrator, 0) ) KillsAsInfiltrator,
         MAX( COALESCE(kill_sums.KillsAsLightAssault, 0) ) KillsAsLightAssault,
         MAX( COALESCE(kill_sums.KillsAsMedic, 0) ) KillsAsMedic,
         MAX( COALESCE(kill_sums.KillsAsEngineer, 0) ) KillsAsEngineer,
         MAX( COALESCE(kill_sums.KillsAsMax, 0) ) KillsAsMax,
         MAX( COALESCE(death_sums.DeathsAsHeavyAssault, 0) ) DeathsAsHeavyAssault,
         MAX( COALESCE(death_sums.DeathsAsInfiltrator, 0) ) DeathsAsInfiltrator,
         MAX( COALESCE(death_sums.DeathsAsLightAssault, 0) ) DeathsAsLightAssault,
         MAX( COALESCE(death_sums.DeathsAsMedic, 0) ) DeathsAsMedic,
         MAX( COALESCE(death_sums.DeathsAsEngineer, 0) ) DeathsAsEngineer,
         MAX( COALESCE(death_sums.DeathsAsMax, 0) ) DeathsAsMax,
         MAX( COALESCE(damage_sums.DamageAssistsAsHeavyAssault, 0) ) DamageAssistsAsHeavyAssault,
         MAX( COALESCE(damage_sums.DamageAssistsAsInfiltrator, 0) ) DamageAssistsAsInfiltrator,
         MAX( COALESCE(damage_sums.DamageAssistsAsLightAssault, 0) ) DamageAssistsAsLightAssault,
         MAX( COALESCE(damage_sums.DamageAssistsAsMedic, 0) ) DamageAssistsAsMedic,
         MAX( COALESCE(damage_sums.DamageAssistsAsEngineer, 0) ) DamageAssistsAsEngineer,
         MAX( COALESCE(damage_sums.DamageAssistsAsMax, 0) ) DamageAssistsAsMax
    FROM ( SELECT match_players.ScrimMatchId,
                  match_players.TeamOrdinal
             FROM [PlanetmansDbContext].[dbo].ScrimMatchParticipatingPlayer match_players
             GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal ) match_teams
      INNER JOIN ( SELECT kills.ScrimMatchId,
                          kills.AttackerTeamOrdinal TeamOrdinal,
                          SUM( kills.Points ) Points,
                          SUM( CASE WHEN DeathType IN ( 0, 1, 2) THEN kills.Points ELSE 0 END ) NetScore,
                          SUM( CASE WHEN DeathType = 0 THEN 1 ELSE 0 END ) Kills,
                          SUM( CASE WHEN DeathType = 1 THEN 1 ELSE 0 END ) TeamKills,
                          SUM( CASE WHEN DeathType = 2 THEN 1 ELSE 0 END ) Suicides,
                          SUM( CASE WHEN DeathType = 0 AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END ) HeadshotKills,
                          SUM( CASE WHEN DeathType = 0 AND kills.Points > 0 THEN 1 ELSE 0 END ) ScoredKills,
                          SUM( CASE WHEN DeathType = 0 AND kills.Points = 0 THEN 1 ELSE 0 END ) ZeroPointKills,
                          SUM( CASE WHEN DeathType = 0 AND kills.AttackerLoadoutId IN ( 1, 8, 15) THEN 1 ELSE 0 END )  KillsAsInfiltrator,
                          SUM( CASE WHEN DeathType = 0 AND kills.AttackerLoadoutId IN ( 3, 10, 17) THEN 1 ELSE 0 END ) KillsAsLightAssault,
                          SUM( CASE WHEN DeathType = 0 AND kills.AttackerLoadoutId IN ( 4, 11, 18) THEN 1 ELSE 0 END ) KillsAsMedic,
                          SUM( CASE WHEN DeathType = 0 AND kills.AttackerLoadoutId IN ( 5, 12, 19) THEN 1 ELSE 0 END ) KillsAsEngineer,
                          SUM( CASE WHEN DeathType = 0 AND kills.AttackerLoadoutId IN ( 6, 13, 20) THEN 1 ELSE 0 END ) KillsAsHeavyAssault,
                          SUM( CASE WHEN DeathType = 0 AND kills.AttackerLoadoutId IN ( 7, 14, 21) THEN 1 ELSE 0 END ) KillsAsMax,
                          SUM( CASE WHEN DeathType = 0 AND ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 ) THEN 1 ELSE 0 END ) AssistedKills,
                          SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END ) DamageAssistedKills,
                          SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END ) DamageAssistedOnlyKills,
                          SUM( CASE WHEN DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END ) GrenadeAssistedKills,
                          SUM( CASE WHEN DeathType = 0 AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END ) GrenadeAssistedOnlyKills,
                          SUM( CASE WHEN DeathType = 0 AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END ) SpotAssistedKills,
                          SUM( CASE WHEN DeathType = 0 AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END ) SpotAssistedOnlyKills,
                          SUM( CASE WHEN DeathType = 0 AND ( damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL ) THEN 1 ELSE 0 END ) UnassistedKills
                      FROM [PlanetmansDbContext].[dbo].ScrimDeath kills
                        LEFT OUTER JOIN ( SELECT ScrimMatchId, damages.AttackerTeamOrdinal, VictimCharacterId, damages.Timestamp, COUNT(*) TotalDamages,
                                            SUM( CASE WHEN damages.ActionType = 304 THEN 1 ELSE 0 END ) EnemyDamages,
                                            SUM( CASE WHEN damages.ActionType = 310 THEN 1 ELSE 0 END ) TeamDamages,
                                            SUM( CASE WHEN damages.ActionType = 312 THEN 1 ELSE 0 END ) SelfDamages
                                      FROM [PlanetmansDbContext].[dbo].ScrimDamageAssist damages
                                      GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId ) damage_sums
                          ON kills.ScrimMatchId = damage_sums.ScrimMatchId
                            AND kills.Timestamp = damage_sums.Timestamp
                            AND kills.AttackerTeamOrdinal = damage_sums.AttackerTeamOrdinal
                            AND kills.VictimCharacterId = damage_sums.VictimCharacterId
                        LEFT OUTER JOIN ( SELECT ScrimMatchId, grenades.AttackerTeamOrdinal, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades,
                                                SUM( CASE WHEN grenades.ActionType = 306 THEN 1 ELSE 0 END ) EnemyGrenades,
                                                SUM( CASE WHEN grenades.ActionType = 311 THEN 1 ELSE 0 END ) TeamGrenades,
                                                SUM( CASE WHEN grenades.ActionType = 313 THEN 1 ELSE 0 END ) SeldGrenades
                                          FROM [PlanetmansDbContext].[dbo].[ScrimGrenadeAssist] grenades
                                          GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId ) grenade_sums
                              ON kills.ScrimMatchId = grenade_sums.ScrimMatchId
                                 AND kills.Timestamp = grenade_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = grenade_sums.AttackerTeamOrdinal
                                 AND kills.VictimCharacterId = grenade_sums.VictimCharacterId
                        LEFT OUTER JOIN ( SELECT ScrimMatchId, spots.SpotterTeamOrdinal, VictimCharacterId, spots.Timestamp, COUNT(*) TotalSpots
                                          FROM [PlanetmansDbContext].[dbo].[ScrimSpotAssist] spots
                                          GROUP BY ScrimMatchId, SpotterTeamOrdinal, Timestamp, VictimCharacterId ) spot_sums
                              ON kills.ScrimMatchId = spot_sums.ScrimMatchId
                                 AND kills.Timestamp = spot_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = spot_sums.SpotterTeamOrdinal
                                 AND kills.VictimCharacterId = spot_sums.VictimCharacterId
                      GROUP BY kills.ScrimMatchId, kills.AttackerTeamOrdinal ) kill_sums
        ON match_teams.ScrimMatchId = kill_sums.ScrimMatchId
           AND match_teams.TeamOrdinal = kill_sums.TeamOrdinal
      LEFT OUTER JOIN ( SELECT deaths.ScrimMatchId,
                               deaths.VictimTeamOrdinal TeamOrdinal,
                               SUM( CASE WHEN DeathType = 0 THEN deaths.Points * -1 ELSE 0 END ) NetScore,
                               SUM( CASE WHEN DeathType IN ( 0, 1, 2) THEN 1 ELSE 0 END ) Deaths,
                               SUM( CASE WHEN DeathType = 1 THEN 1 ELSE 0 END ) TeamKillDeaths,
                               SUM( CASE WHEN DeathType = 2 THEN 1 ELSE 0 END ) Suicides,
                               SUM( CASE WHEN DeathType = 0 AND deaths.IsHeadshot = 1 THEN 1 ELSE 0 END ) HeadshotEnemyDeaths,
                               SUM( CASE WHEN deaths.IsHeadshot = 1 THEN 1 ELSE 0 END ) HeadshotDeaths,
                               SUM( CASE WHEN deaths.Points > 0 THEN 1 ELSE 0 END ) ScoredDeaths,
                               SUM( CASE WHEN deaths.Points = 0 THEN 1 ELSE 0 END ) ZeroPointDeaths,
                               SUM( CASE WHEN deaths.VictimLoadoutId IN ( 1, 8, 15) THEN 1 ELSE 0 END )  DeathsAsInfiltrator,
                               SUM( CASE WHEN deaths.VictimLoadoutId IN ( 3, 10, 17) THEN 1 ELSE 0 END ) DeathsAsLightAssault,
                               SUM( CASE WHEN deaths.VictimLoadoutId IN ( 4, 11, 18) THEN 1 ELSE 0 END ) DeathsAsMedic,
                               SUM( CASE WHEN deaths.VictimLoadoutId IN ( 5, 12, 19) THEN 1 ELSE 0 END ) DeathsAsEngineer,
                               SUM( CASE WHEN deaths.VictimLoadoutId IN ( 6, 13, 20) THEN 1 ELSE 0 END ) DeathsAsHeavyAssault,
                               SUM( CASE WHEN deaths.VictimLoadoutId IN ( 7, 14, 21) THEN 1 ELSE 0 END ) DeathsAsMax,
                               SUM( CASE WHEN DeathType = 0 AND ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 ) THEN 1 ELSE 0 END ) AssistedEnemyDeaths,
                                SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END ) DamageAssistedEnemyDeaths,
                                SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END ) DamageAssistedOnlyEnemyDeaths,
                                SUM( CASE WHEN DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END ) GrenadeAssistedEnemyDeaths,
                                SUM( CASE WHEN DeathType = 0 AND grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END ) GrenadeAssistedOnlyEnemyDeaths,
                                SUM( CASE WHEN DeathType = 0 AND spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END ) SpotAssistedEnemyDeaths,
                                SUM( CASE WHEN DeathType = 0 AND spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END ) SpotAssistedOnlyEnemyDeaths,
                                SUM( CASE WHEN damage_sums.TotalDamages > 0 THEN 1 ELSE 0 END ) DamageAssistedDeaths,
                                SUM( CASE WHEN damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END ) DamageAssistedOnlyDeaths,
                                SUM( CASE WHEN grenade_sums.TotalGrenades > 0 THEN 1 ELSE 0 END ) GrenadeAssistedDeaths,
                                SUM( CASE WHEN grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0) THEN 1 ELSE 0 END ) GrenadeAssistedOnlyDeaths,
                                SUM( CASE WHEN spot_sums.TotalSpots > 0 THEN 1 ELSE 0 END ) SpotAssistedDeaths,
                                SUM( CASE WHEN spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0) THEN 1 ELSE 0 END ) SpotAssistedOnlyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1 ELSE 0 END ) UnassistedEnemyDeaths,
                               SUM( CASE WHEN damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1 ELSE 0 END ) UnassistedDeaths
                           FROM [PlanetmansDbContext].[dbo].ScrimDeath deaths
                             LEFT OUTER JOIN ( SELECT ScrimMatchId, damages.VictimTeamOrdinal, VictimCharacterId, damages.Timestamp, COUNT(*) TotalDamages,
                                                 SUM( CASE WHEN damages.ActionType = 304 THEN 1 ELSE 0 END ) EnemyDamages,
                                                 SUM( CASE WHEN damages.ActionType = 310 THEN 1 ELSE 0 END ) TeamDamages,
                                                 SUM( CASE WHEN damages.ActionType = 312 THEN 1 ELSE 0 END ) SelfDamages
                                           FROM [PlanetmansDbContext].[dbo].ScrimDamageAssist damages
                                           GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) damage_sums
                               ON deaths.ScrimMatchId = damage_sums.ScrimMatchId
                                 AND deaths.Timestamp = damage_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = damage_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = damage_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, grenades.VictimTeamOrdinal, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades,
                                                SUM( CASE WHEN grenades.ActionType = 306 THEN 1 ELSE 0 END ) EnemyGrenades,
                                                SUM( CASE WHEN grenades.ActionType = 311 THEN 1 ELSE 0 END ) TeamGrenades,
                                                SUM( CASE WHEN grenades.ActionType = 313 THEN 1 ELSE 0 END ) SelfGrenades
                                          FROM [PlanetmansDbContext].[dbo].[ScrimGrenadeAssist] grenades
                                          GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) grenade_sums
                              ON deaths.ScrimMatchId = grenade_sums.ScrimMatchId
                                 AND deaths.Timestamp = grenade_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = grenade_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = grenade_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, spots.VictimTeamOrdinal, VictimCharacterId, spots.Timestamp, COUNT(*) TotalSpots
                                          FROM [PlanetmansDbContext].[dbo].[ScrimSpotAssist] spots
                                          GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) spot_sums
                              ON deaths.ScrimMatchId = spot_sums.ScrimMatchId
                                 AND deaths.Timestamp = spot_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = spot_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = spot_sums.VictimCharacterId
                           GROUP BY deaths.ScrimMatchId, deaths.VictimTeamOrdinal ) death_sums
        ON kill_sums.ScrimMatchId = death_sums.ScrimMatchId
          AND kill_sums.TeamOrdinal = death_sums.TeamOrdinal
          AND match_teams.ScrimMatchId = death_sums.ScrimMatchId
          AND match_teams.TeamOrdinal = death_sums.TeamOrdinal
      LEFT OUTER JOIN ( SELECT match_players.ScrimMatchId,
                               match_players.TeamOrdinal,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 THEN 1 ELSE 0 END ) DamageAssists,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 310 THEN 1 ELSE 0 END ) DamageTeamAssists,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN ( 1, 8, 15) THEN 1 ELSE 0 END )  DamageAssistsAsInfiltrator,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN ( 3, 10, 17) THEN 1 ELSE 0 END ) DamageAssistsAsLightAssault,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN ( 4, 11, 18) THEN 1 ELSE 0 END ) DamageAssistsAsMedic,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN ( 5, 12, 19) THEN 1 ELSE 0 END ) DamageAssistsAsEngineer,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN ( 6, 13, 20) THEN 1 ELSE 0 END ) DamageAssistsAsHeavyAssault,
                               SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 AND damages.AttackerLoadoutId IN ( 7, 14, 21) THEN 1 ELSE 0 END ) DamageAssistsAsMax
                         FROM [PlanetmansDbContext].[dbo].ScrimMatchParticipatingPlayer match_players
                           INNER JOIN [PlanetmansDbContext].[dbo].ScrimDamageAssist damages
                             ON match_players.ScrimMatchId = damages.ScrimMatchId
                                 AND  match_players.CharacterId = damages.AttackerCharacterId
                         GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal ) damage_sums
                ON damage_sums.ScrimMatchId = match_teams.ScrimMatchId
                  AND damage_sums.TeamOrdinal = match_teams.TeamOrdinal
      LEFT OUTER JOIN ( SELECT match_players.ScrimMatchId,
                               match_players.TeamOrdinal,
                               SUM( CASE WHEN CharacterId = grenades.AttackerCharacterId AND grenades.ActionType = 306 THEN 1 ELSE 0 END ) GrenadeAssistedKills,
                               SUM( CASE WHEN CharacterId = grenades.AttackerCharacterId AND grenades.ActionType = 311 THEN 1 ELSE 0 END ) GrenadeTeamAssistedKills
                         FROM [PlanetmansDbContext].[dbo].ScrimMatchParticipatingPlayer match_players
                           INNER JOIN [PlanetmansDbContext].[dbo].ScrimGrenadeAssist grenades
                             ON match_players.ScrimMatchId = grenades.ScrimMatchId
                                 AND  match_players.CharacterId = grenades.AttackerCharacterId
                         GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal ) grenade_sums
                ON grenade_sums.ScrimMatchId = match_teams.ScrimMatchId
                  AND grenade_sums.TeamOrdinal = match_teams.TeamOrdinal
      LEFT OUTER JOIN ( SELECT match_players.ScrimMatchId,
                               match_players.TeamOrdinal,
                               SUM( CASE WHEN CharacterId = spots.SpotterCharacterId AND spots.ActionType = 308 THEN 1 ELSE 0 END ) SpotAssistedKills
                         FROM [PlanetmansDbContext].[dbo].ScrimMatchParticipatingPlayer match_players
                           INNER JOIN [PlanetmansDbContext].[dbo].ScrimSpotAssist spots
                             ON match_players.ScrimMatchId = spots.ScrimMatchId
                                 AND  match_players.CharacterId = spots.SpotterCharacterId
                         GROUP BY match_players.ScrimMatchId, match_players.TeamOrdinal ) spot_sums
                ON spot_sums.ScrimMatchId = match_teams.ScrimMatchId
                  AND spot_sums.TeamOrdinal = match_teams.TeamOrdinal
      LEFT OUTER JOIN ( SELECT adjustments.ScrimMatchId,
                               adjustments.TeamOrdinal,
                               SUM( Points ) Points
                          FROM [PlanetmansDbContext].[dbo].ScrimMatchTeamPointAdjustment adjustments
                          GROUP BY adjustments.ScrimMatchId, adjustments.TeamOrdinal) adjustment_sums
        ON adjustment_sums.ScrimMatchId = match_teams.ScrimMatchId
           AND adjustment_sums.TeamOrdinal = match_teams.TeamOrdinal
      LEFT OUTER JOIN ( SELECT captures.ScrimMatchId,
                               captures.ControllingTeamOrdinal TeamOrdinal,
                               SUM( captures.Points ) Points
                          FROM [PlanetmansDbContext].[dbo].ScrimFacilityControl captures
                          GROUP BY captures.ScrimMatchId, captures.ControllingTeamOrdinal ) capture_sums
        ON match_teams.ScrimMatchId = capture_sums.ScrimMatchId
           AND match_teams.TeamOrdinal = capture_sums.TeamOrdinal
    GROUP BY match_teams.ScrimMatchId, match_teams.TeamOrdinal
