USE [PlanetmansDbContext];

GO

CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerStats AS

  SELECT ScrimMatchId,
          CharacterId,
          TeamOrdinal,
          MAX(NameDisplay) NameDisplay,
          MAX(NameFull) NameFull,
          MAX(FactionId) FactionId,
          MAX(WorldId) WorldId,
          MAX(PrestigeLevel) PrestigeLevel,
          SUM( CASE WHEN CharacterId = AttackerCharacterId
                      THEN Points
                    ELSE 0 END ) Points,
          SUM( CASE WHEN DeathType = 0 -- Kill
                      THEN CASE WHEN CharacterId = AttackerCharacterId
                                  THEN Points
                                ELSE (Points * -1) END
                    WHEN DeathType = 1 -- Teamkill
                      THEN CASE WHEN CharacterId = AttackerCharacterId
                                  THEN Points
                                ELSE 0 END
                    WHEN DeathType = 2 -- Suicide
                      THEN CASE WHEN CharacterId = VictimCharacterId
                                  THEN Points
                                ELSE 0 END
                    ELSE 0 END ) NetScore,
          SUM( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0
                      THEN 1
                    ELSE 0 END ) Kills,
          SUM( CASE WHEN CharacterId = VictimCharacterId
                      THEN 1
                    ELSE 0 END ) Deaths,
          SUM( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 1
                      THEN 1
                    ELSE 0 END ) TeamKills,
          SUM( CASE WHEN CharacterId = VictimCharacterId AND DeathType = 2
                      THEN 1
                    ELSE 0 END ) Suicides,
          SUM( CASE WHEN CharacterId = VictimCharacterId AND DeathType = 0 AND Points > 0
                      THEN 1
                    ELSE 0 END ) ScoredDeaths,
          SUM( CASE WHEN CharacterId = VictimCharacterId AND DeathType = 0 AND Points = 0
                      THEN 1
                    ELSE 0 END ) ZeroPointDeaths,
          SUM( CASE WHEN CharacterId = VictimCharacterId AND DeathType = 1
                      THEN 1
                    ELSE 0 END ) TeamKillDeaths,
          SUM( CASE WHEN CharacterId = DamageAttackerCharacterId AND DamageActionType = 304 -- DamageAssist
                      THEN 1
                    ELSE 0 END ) DamageAssists,
          SUM( CASE WHEN CharacterId = DamageAttackerCharacterId AND DamageActionType = 310 -- DamageTeamAssist
                      THEN 1
                    ELSE 0 END ) DamageTeamAssists,
          SUM( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0 AND DamageActionType = 304 --DamageAttackerCharacterId IS NOT NULL --DamageActionType = 304 -- DamageAttackerCharacterId IS NOT NULL AND DamageAttackerCharacterId <> CharacterId
                      THEN 1
                    ELSE 0 END ) DamageAssistedKills,
          SUM( CASE WHEN CharacterId = VictimCharacterId AND CharacterId = DamageVictimCharacterId
                      THEN 1
                    ELSE 0 END ) DamageAssistedDeaths,
          SUM( CASE WHEN CharacterId = VictimCharacterId AND DeathType = 0 AND CharacterId = DamageVictimCharacterId
                      THEN 1
                    ELSE 0 END ) DamageAssistedEnemyDeaths,
          SUM( CASE WHEN CharacterId = VictimCharacterId AND DeathType = 0 AND DamageActionType IS NULL
                      THEN 1
                    ELSE 0 END ) UnassistedEnemyDeaths,
          SUM ( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0 AND AttackerLoadoutId IN ( 7, 15, 22)
                        THEN 1
                      ELSE 0 END ) KillsAsHeavyAssault,
          SUM ( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0 AND AttackerLoadoutId IN ( 2, 10, 17)
                        THEN 1
                      ELSE 0 END ) KillsAsInfiltrator,
          SUM ( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0 AND AttackerLoadoutId IN ( 4, 12, 19)
                        THEN 1
                      ELSE 0 END ) KillsAsLightAssault,
          SUM ( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0 AND AttackerLoadoutId IN ( 5, 13, 20)
                        THEN 1
                      ELSE 0 END ) KillsAsMedic,
          SUM ( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0 AND AttackerLoadoutId IN ( 6, 14, 21)
                        THEN 1
                      ELSE 0 END ) KillsAsEngineer,
          SUM ( CASE WHEN CharacterId = AttackerCharacterId AND DeathType = 0 AND AttackerLoadoutId IN ( 8, 16, 23)
                        THEN 1
                      ELSE 0 END ) KillsAsMax,
          SUM ( CASE WHEN CharacterId = VictimCharacterId AND AttackerLoadoutId IN ( 7, 15, 22)
                        THEN 1
                      ELSE 0 END ) DeathsAsHeavyAssault,
          SUM ( CASE WHEN CharacterId = VictimCharacterId AND AttackerLoadoutId IN ( 2, 10, 17)
                        THEN 1
                      ELSE 0 END ) DeathsAsInfiltrator,
          SUM ( CASE WHEN CharacterId = VictimCharacterId AND AttackerLoadoutId IN ( 4, 12, 19)
                        THEN 1
                      ELSE 0 END ) DeathsAsLightAssault,
          SUM ( CASE WHEN CharacterId = VictimCharacterId AND AttackerLoadoutId IN ( 5, 13, 20)
                        THEN 1
                      ELSE 0 END ) DeathsAsMedic,
          SUM ( CASE WHEN CharacterId = VictimCharacterId AND AttackerLoadoutId IN ( 6, 14, 21)
                        THEN 1
                      ELSE 0 END ) DeathsAsEngineer,
          SUM ( CASE WHEN CharacterId = VictimCharacterId AND AttackerLoadoutId IN ( 8, 16, 23)
                        THEN 1
                      ELSE 0 END ) DeathsAsMax
    FROM ( SELECT match_players.ScrimMatchId,
                  match_players.CharacterId,
                  match_players.TeamOrdinal,
                  MAX(match_players.NameDisplay) NameDisplay,
                  MAX(match_players.NameFull) NameFull,
                  MAX(match_players.FactionId) FactionId,
                  MAX(match_players.WorldId) WorldId,
                  MAX(match_players.PrestigeLevel) PrestigeLevel,
                  kills.Timestamp,
                  kills.AttackerCharacterId,
                  kills.VictimCharacterId,
                  MAX(kills.AttackerLoadoutId) AttackerLoadoutId,
                  MAX(kills.VictimLoadoutId) VictimLoadoutId,
                  MAX(kills.AttackerTeamOrdinal) AttackerTeamOrdinal,
                  MAX(kills.VictimTeamOrdinal) VictimTeamOrdinal,
                  MAX(DeathType) DeathType,
                  MAX(kills.Points) Points,
                  damages.AttackerCharacterId DamageAttackerCharacterId,
                  damages.VictimCharacterId DamageVictimCharacterId,
                  MAX(damages.AttackerTeamOrdinal) DamageAttackerTeamOrdinal,
                  MAX(damages.VictimTeamOrdinal) DamageVictimTeamOrdinal,
                  MAX(damages.ActionType) DamageActionType,
                  MAX(damages.ExperienceGainAmount) DamageExperienceGainAmount
            FROM ScrimMatchParticipatingPlayer match_players
              INNER JOIN ScrimDeath kills
                ON match_players.ScrimMatchId = kills.ScrimMatchId
                    AND ( kills.AttackerCharacterId = match_players.CharacterId
                          OR kills.VictimCharacterId = match_players.CharacterId )
              LEFT OUTER JOIN ScrimDamageAssist damages
                ON kills.ScrimMatchId = damages.ScrimMatchId
                    AND kills.Timestamp = damages.Timestamp
                    AND ( kills.VictimCharacterId = damages.VictimCharacterId
                          OR damages.AttackerCharacterID = match_players.CharacterId)
            GROUP BY match_players.ScrimMatchId, match_players.CharacterId, match_players.TeamOrdinal, kills.Timestamp, kills.AttackerCharacterId, kills.VictimCharacterId, damages.AttackerCharacterId, damages.VictimCharacterId ) grouped_events
    GROUP BY ScrimMatchId, CharacterId, TeamOrdinal
    --ORDER BY ScrimMatchId, TeamOrdinal, CharacterId