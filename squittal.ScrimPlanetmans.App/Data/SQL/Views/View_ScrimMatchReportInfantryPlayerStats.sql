USE [PlanetmansDbContext];

GO

CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerStats AS

  SELECT match_players.ScrimMatchId,
             match_players.TeamOrdinal,
             match_players.CharacterId,
             match_players.NameDisplay,
             match_players.NameFull,
             match_players.FactionId,
             match_players.WorldId,
             match_players.PrestigeLevel,
             --match_players.IsFromOutfit,
             --match_players.OutfitId,
             --match_players.OutfitAlias,
             --match_players.IsFromConstructedTeam,
             --match_players.ConstructedTeamId,
             killStats.Points,
             killStats.NetScore,
             killStats.Kills,
             killStats.Deaths,
             killStats.TeamKills,
             killStats.Suicides,
             killStats.HeadshotKills,
             killStats.HeadshotEnemyDeaths,
             killStats.ScoredDeaths,
             killStats.ZeroPointDeaths,
             killStats.TeamKillDeaths,
             killStats.KillsAsHeavyAssault,
             killStats.KillsAsInfiltrator,
             killStats.KillsAsLightAssault,
             killStats.KillsAsMedic,
             killStats.KillsAsEngineer,
             killStats.KillsAsMax,
             killStats.DeathsAsHeavyAssault,
             killStats.DeathsAsInfiltrator,
             killStats.DeathsAsLightAssault,
             killStats.DeathsAsMedic,
             killStats.DeathsAsEngineer,
             killStats.DeathsAsMax,
             killStats.DamageAssistedKills,
             killStats.DamageAssistedDeaths,
             killStats.DamageAssistedEnemyDeaths,
             killStats.UnassistedEnemyDeaths,
             damageStats.DamageAssists,
             damageStats.DamageTeamAssists,
             damageStats.DamageAssistsAsHeavyAssault,
             damageStats.DamageAssistsAsInfiltrator,
             damageStats.DamageAssistsAsLightAssault,
             damageStats.DamageAssistsAsMedic,
             damageStats.DamageAssistsAsEngineer,
             damageStats.DamageAssistsAsMax
        FROM ScrimMatchParticipatingPlayer match_players
          INNER JOIN ( SELECT match_players.ScrimMatchId, match_players.CharacterId,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId THEN kills.Points ELSE 0 END ) Points,
                              SUM( CASE WHEN DeathType = 0 -- Kill
                                          THEN CASE WHEN CharacterId = kills.AttackerCharacterId THEN kills.Points ELSE (kills.Points * -1) END
                                        WHEN DeathType = 1 -- Teamkill
                                          THEN CASE WHEN CharacterId = kills.AttackerCharacterId THEN kills.Points ELSE 0 END
                                        WHEN DeathType = 2 -- Suicide
                                          THEN CASE WHEN CharacterId = kills.VictimCharacterId THEN kills.Points ELSE 0 END
                                        ELSE 0 END ) NetScore,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 THEN 1 ELSE 0 END ) Kills,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId THEN 1 ELSE 0 END ) Deaths,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 1 THEN 1 ELSE 0 END ) TeamKills,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 2 THEN 1 ELSE 0 END ) Suicides,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END ) HeadshotKills,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND kills.IsHeadshot = 1 THEN 1 ELSE 0 END ) HeadshotEnemyDeaths,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND kills.Points > 0 THEN 1 ELSE 0 END ) ScoredDeaths,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND kills.Points = 0 THEN 1 ELSE 0 END ) ZeroPointDeaths,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 1 THEN 1 ELSE 0 END ) TeamKillDeaths,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN ( 7, 15, 22) THEN 1 ELSE 0 END ) KillsAsHeavyAssault,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN ( 2, 10, 17) THEN 1 ELSE 0 END ) KillsAsInfiltrator,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN ( 4, 12, 19) THEN 1 ELSE 0 END ) KillsAsLightAssault,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN ( 5, 13, 20) THEN 1 ELSE 0 END ) KillsAsMedic,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN ( 6, 14, 21) THEN 1 ELSE 0 END ) KillsAsEngineer,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND kills.AttackerLoadoutId IN ( 8, 16, 23) THEN 1 ELSE 0 END ) KillsAsMax,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND kills.AttackerLoadoutId IN ( 7, 15, 22) THEN 1 ELSE 0 END ) DeathsAsHeavyAssault,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND kills.AttackerLoadoutId IN ( 2, 10, 17) THEN 1 ELSE 0 END ) DeathsAsInfiltrator,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND kills.AttackerLoadoutId IN ( 4, 12, 19) THEN 1 ELSE 0 END ) DeathsAsLightAssault,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND kills.AttackerLoadoutId IN ( 5, 13, 20) THEN 1 ELSE 0 END ) DeathsAsMedic,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND kills.AttackerLoadoutId IN ( 6, 14, 21) THEN 1 ELSE 0 END ) DeathsAsEngineer,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND kills.AttackerLoadoutId IN ( 8, 16, 23) THEN 1 ELSE 0 END ) DeathsAsMax,
                              SUM( CASE WHEN CharacterId = kills.AttackerCharacterId AND DeathType = 0 AND damages.ActionType = 304 THEN 1 ELSE 0 END ) DamageAssistedKills,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND CharacterId = damages.VictimCharacterId THEN 1 ELSE 0 END ) DamageAssistedDeaths,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND CharacterId = damages.VictimCharacterId THEN 1 ELSE 0 END ) DamageAssistedEnemyDeaths,
                              SUM( CASE WHEN CharacterId = kills.VictimCharacterId AND DeathType = 0 AND damages.ActionType IS NULL THEN 1 ELSE 0 END ) UnassistedEnemyDeaths
                          FROM ScrimMatchParticipatingPlayer match_players
                            INNER JOIN ScrimDeath kills
                              ON match_players.ScrimMatchId = kills.ScrimMatchId
                                AND ( kills.AttackerCharacterId = match_players.CharacterId
                                      OR kills.VictimCharacterId = match_players.CharacterId )
                            LEFT OUTER JOIN ScrimDamageAssist damages
                              ON kills.ScrimMatchId = damages.ScrimMatchId
                                AND kills.Timestamp = damages.Timestamp
                          GROUP BY match_players.ScrimMatchId, match_players.CharacterId
                      ) killStats
            ON killStats.ScrimMatchId = match_players.ScrimMatchId
                AND killStats.CharacterId = match_players.CharacterId
          LEFT OUTER JOIN ( SELECT match_players.ScrimMatchId,
                                   match_players.CharacterId,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 304 THEN 1 ELSE 0 END ) DamageAssists,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.ActionType = 310 THEN 1 ELSE 0 END ) DamageTeamAssists,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN ( 7, 15, 22) THEN 1 ELSE 0 END ) DamageAssistsAsHeavyAssault,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN ( 2, 10, 17) THEN 1 ELSE 0 END ) DamageAssistsAsInfiltrator,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN ( 4, 12, 19) THEN 1 ELSE 0 END ) DamageAssistsAsLightAssault,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN ( 5, 13, 20) THEN 1 ELSE 0 END ) DamageAssistsAsMedic,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN ( 6, 14, 21) THEN 1 ELSE 0 END ) DamageAssistsAsEngineer,
                                   SUM( CASE WHEN CharacterId = damages.AttackerCharacterId AND damages.AttackerLoadoutId IN ( 8, 16, 23) THEN 1 ELSE 0 END ) DamageAssistsAsMax
                              FROM ScrimMatchParticipatingPlayer match_players
                                INNER JOIN ScrimDamageAssist damages
                                  ON match_players.ScrimMatchId = damages.ScrimMatchId
                                     AND  match_players.CharacterId = damages.AttackerCharacterId
                              GROUP BY match_players.ScrimMatchId, match_players.CharacterId
                          ) damageStats
              ON damageStats.ScrimMatchId = match_players.ScrimMatchId
                AND damageStats.CharacterId = match_players.CharacterId