USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchReportInfantryPlayerHeadToHeadStats'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchReportInfantryPlayerHeadToHeadStats as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchReportInfantryPlayerHeadToHeadStats AS
-- CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerHeadToHeadStats AS

  SELECT match_players.ScrimMatchId,
         match_players.TeamOrdinal PlayerTeamOrdinal,
         match_players.CharacterId PlayerCharacterId,
         match_players.NameDisplay PlayerNameDisplay,
         match_players.NameFull PlayerNameFull,
         match_players.FactionId PlayerFactionId,
         match_players.PrestigeLevel PlayerPrestigeLevel,
         match_players2.TeamOrdinal OpponentTeamOrdinal,
         match_players2.CharacterId OpponentCharacterId,
         match_players2.NameDisplay OpponentNameDisplay,
         match_players2.NameFull OpponentNameFull,
         match_players2.FactionId OpponentFactionId,
         match_players2.PrestigeLevel OpponentPrestigeLevel,
         COALESCE(kill_sums.Points, 0) Points,
         COALESCE(kill_sums.NetScore, 0) + COALESCE(death_sums.NetScore, 0) NetScore,
         COALESCE(kill_sums.Kills, 0) Kills,
         COALESCE(kill_sums.HeadshotKills, 0) HeadshotKills,
         COALESCE(death_sums.Deaths, 0) Deaths,
         COALESCE(death_sums.HeadshotDeaths, 0) HeadshotDeaths,
         COALESCE(death_sums.ScoredDeaths, 0) ScoredDeaths,
         COALESCE(death_sums.ZeroPointDeaths, 0) ZeroPointDeaths,
         COALESCE(damages_dealt.TotalDamageAssistsDealt, 0) DamageAssistsDealt,
         COALESCE(damages_taken.TotalDamageAssistsTaken, 0) DamageAssistsTaken,
         COALESCE(kill_sums.DamageAssistedKills, 0) DamageAssistedKills,
         COALESCE(kill_sums.DamageAssistedOnlyKills, 0) DamageAssistedOnlyKills,
         COALESCE(kill_sums.GrenadeAssistedKills, 0) GrenadeAssistedKills,
         COALESCE(kill_sums.GrenadeAssistedOnlyKills, 0) GrenadeAssistedOnlyKills,
         COALESCE(kill_sums.SpotAssistedKills, 0) SpotAssistedKills,
         COALESCE(kill_sums.SpotAssistedOnlyKills, 0) SpotAssistedOnlyKills,
         COALESCE(kill_sums.AssistedKills, 0) AssistedKills,
         COALESCE(kill_sums.UnassistedKills, 0) UnassistedKills,
         COALESCE(kill_sums.UnassistedHeadshotKills, 0) UnassistedHeadshotKills,
         COALESCE(death_sums.DamageAssistedDeaths, 0) DamageAssistedDeaths,
         COALESCE(death_sums.DamageAssistedOnlyDeaths, 0) DamageAssistedOnlyDeaths,
         COALESCE(death_sums.GrenadeAssistedDeaths, 0) GrenadeAssistedDeaths,
         COALESCE(death_sums.GrenadeAssistedOnlyDeaths, 0) GrenadeAssistedOnlyDeaths,
         COALESCE(death_sums.SpotAssistedDeaths, 0) SpotAssistedDeaths,
         COALESCE(death_sums.SpotAssistedOnlyDeaths, 0) SpotAssistedOnlyDeaths,
         COALESCE(death_sums.AssistedDeaths, 0) AssistedDeaths,
         COALESCE(death_sums.UnassistedDeaths, 0) UnassistedDeaths,
         COALESCE(death_sums.UnassistedHeadshotDeaths, 0) UnassistedHeadshotDeaths
    FROM [dbo].ScrimMatchParticipatingPlayer match_players
      FULL OUTER JOIN [dbo].ScrimMatchParticipatingPlayer match_players2
        ON match_players.ScrimMatchId = match_players2.ScrimMatchId
            AND ( match_players.CharacterId <> match_players2.CharacterId )
      LEFT OUTER JOIN ( SELECT deaths.ScrimMatchId,
                               deaths.VictimCharacterId CharacterId,
                               deaths.AttackerCharacterId DeathAttackerCharacterId,
                               MAX( deaths.VictimTeamOrdinal ) TeamOrdinal,
                               MAX( attacker_players.TeamOrdinal ) DeathAttackerTeamOrdinal,
                               MAX( attacker_players.NameDisplay ) DeathAttackerNameDisplay,
                               MAX( attacker_players.NameFull ) DeathAttackerNameFull,
                               MAX( attacker_players.FactionId ) DeathAttackerFactionId,
                               MAX( attacker_players.PrestigeLevel ) DeathAttackerPrestigeLevel,
                               SUM( CASE WHEN DeathType = 0
                                           THEN deaths.Points * -1
                                         ELSE 0 END ) NetScore,
                               SUM( 1 ) Deaths,
                               SUM( CASE WHEN deaths.IsHeadshot = 1
                                           THEN 1
                                         ELSE 0 END ) HeadshotDeaths,
                               SUM( CASE WHEN deaths.Points <> 0
                                           THEN 1
                                         ELSE 0 END ) ScoredDeaths,
                               SUM( CASE WHEN deaths.Points = 0
                                           THEN 1
                                         ELSE 0 END ) ZeroPointDeaths,
                               SUM( CASE WHEN ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 )
                                           THEN 1
                                         ELSE 0 END ) AssistedDeaths,
                               SUM( CASE WHEN damage_sums.TotalDamages > 0
                                           THEN 1
                                         ELSE 0 END ) DamageAssistedDeaths,
                               SUM( CASE WHEN damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0)
                                           THEN 1
                                         ELSE 0 END ) DamageAssistedOnlyDeaths,
                               SUM( CASE WHEN grenade_sums.TotalGrenades > 0
                                           THEN 1
                                         ELSE 0 END ) GrenadeAssistedDeaths,
                               SUM( CASE WHEN grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0)
                                           THEN 1
                                         ELSE 0 END ) GrenadeAssistedOnlyDeaths,
                               SUM( CASE WHEN spot_sums.TotalSpots > 0
                                           THEN 1
                                         ELSE 0 END ) SpotAssistedDeaths,
                               SUM( CASE WHEN spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0)
                                           THEN 1
                                         ELSE 0 END ) SpotAssistedOnlyDeaths,
                               SUM( CASE WHEN damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedDeaths,
                               SUM( CASE WHEN deaths.IsHeadshot = 1 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedHeadshotDeaths
                          FROM [dbo].ScrimDeath deaths
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, damages.VictimTeamOrdinal, VictimCharacterId, damages.Timestamp, COUNT(*) TotalDamages
                                                FROM [dbo].ScrimDamageAssist damages
                                                GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) damage_sums
                              ON deaths.ScrimMatchId = damage_sums.ScrimMatchId
                                 AND deaths.Timestamp = damage_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = damage_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = damage_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, grenades.VictimTeamOrdinal, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades
                                                FROM [dbo].[ScrimGrenadeAssist] grenades
                                                GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) grenade_sums
                              ON deaths.ScrimMatchId = grenade_sums.ScrimMatchId
                                 AND deaths.Timestamp = grenade_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = grenade_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = grenade_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, spots.VictimTeamOrdinal, VictimCharacterId, spots.Timestamp, COUNT(*) TotalSpots
                                                FROM [dbo].[ScrimSpotAssist] spots
                                                GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) spot_sums
                              ON deaths.ScrimMatchId = spot_sums.ScrimMatchId
                                 AND deaths.Timestamp = spot_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = spot_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = spot_sums.VictimCharacterId
                            LEFT OUTER JOIN [dbo].ScrimMatchParticipatingPlayer attacker_players
                              ON attacker_players.ScrimMatchId = deaths.ScrimMatchId
                                 AND attacker_players.CharacterId = deaths.AttackerCharacterId
                        GROUP BY deaths.ScrimMatchId, deaths.VictimCharacterId, deaths.AttackerCharacterId ) death_sums
        ON match_players.ScrimMatchId = death_sums.ScrimMatchId
           AND match_players.TeamOrdinal = death_sums.TeamOrdinal
           AND match_players.CharacterId = death_sums.CharacterId
           AND match_players2.CharacterId = death_sums.DeathAttackerCharacterId
      LEFT OUTER JOIN ( SELECT match_players.ScrimMatchId DamageDealtScrimMatchId,
                               match_players.CharacterId DamageDealtAttackerCharacterId,
                               damages.VictimCharacterId DamageDealtVictimCharacterId,
                               MAX( victim_players.TeamOrdinal ) DamageDealtVictimTeamOrdinal,
                               MAX( victim_players.NameDisplay ) DamageDealtVictimNameDisplay,
                               MAX( victim_players.NameFull ) DamageDealtVictimNameFull,
                               MAX( victim_players.FactionId ) DamageDealtVictimFactionId,
                               MAX( victim_players.PrestigeLevel ) DamageDealtVictimPrestigeLevel,
                               SUM( CASE WHEN match_players.CharacterId = damages.AttackerCharacterId
                                           THEN 1
                                         ELSE 0 END ) TotalDamageAssistsDealt
                          FROM [dbo].ScrimMatchParticipatingPlayer match_players
                            LEFT OUTER JOIN [dbo].ScrimDamageAssist damages
                              ON match_players.ScrimMatchId = damages.ScrimMatchId
                                  AND match_players.CharacterId = damages.AttackerCharacterId
                            INNER JOIN [dbo].ScrimMatchParticipatingPlayer victim_players
                              ON victim_players.ScrimMatchId = damages.ScrimMatchId
                                  AND victim_players.CharacterId = damages.VictimCharacterId
                          GROUP BY match_players.ScrimMatchId, match_players.CharacterId, damages.VictimCharacterId ) damages_dealt
        ON match_players.ScrimMatchId = damages_dealt.DamageDealtScrimMatchId
           AND match_players.CharacterId = damages_dealt.DamageDealtAttackerCharacterId
           AND match_players2.CharacterId = damages_dealt.DamageDealtVictimCharacterId
      LEFT OUTER JOIN ( SELECT match_players.ScrimMatchId DamageTakenScrimMatchId,
                               match_players.CharacterId DamageTakenVictimCharacterId,
                               damages.AttackerCharacterId DamageTakenAttackerCharacterId,
                               MAX( attacker_players.TeamOrdinal ) DamageTakenAttackerTeamOrdinal,
                               MAX( attacker_players.NameDisplay ) DamageTakenAttackerNameDisplay,
                               MAX( attacker_players.NameFull ) DamageTakenAttackerNameFull,
                               MAX( attacker_players.FactionId ) DamageTakenAttackerFactionId,
                               MAX( attacker_players.PrestigeLevel ) DamageTakenAttackerPrestigeLevel,
                               SUM( CASE WHEN match_players.CharacterId = damages.VictimCharacterId
                                           THEN 1
                                         ELSE 0 END ) TotalDamageAssistsTaken
                          FROM [dbo].ScrimMatchParticipatingPlayer match_players
                            LEFT OUTER JOIN [dbo].ScrimDamageAssist damages
                              ON match_players.ScrimMatchId = damages.ScrimMatchId
                                 AND match_players.CharacterId = damages.VictimCharacterId
                            INNER JOIN [dbo].ScrimMatchParticipatingPlayer attacker_players
                              ON attacker_players.ScrimMatchId = damages.ScrimMatchId
                                 AND attacker_players.CharacterId = damages.AttackerCharacterId
                          GROUP BY match_players.ScrimMatchId, match_players.CharacterId, damages.AttackerCharacterId ) damages_taken
        ON damages_taken.DamageTakenScrimMatchId = match_players.ScrimMatchId
           AND match_players.CharacterId = damages_taken.DamageTakenVictimCharacterId
           AND match_players2.CharacterId = damages_taken.DamageTakenAttackerCharacterId
      LEFT OUTER JOIN ( SELECT kills.ScrimMatchId,
                               kills.AttackerCharacterId CharacterId,
                               kills.VictimCharacterId KillVictimCharacterId,
                               MAX( kills.AttackerTeamOrdinal ) TeamOrdinal,
                               MAX( victim_players.TeamOrdinal ) KillVictimTeamOrdinal,
                               MAX( victim_players.NameDisplay ) KillVictimNameDisplay,
                               MAX( victim_players.NameFull ) KillVictimNameFull,
                               MAX( victim_players.FactionId ) KillVictimFactionId,
                               MAX( victim_players.PrestigeLevel ) KillVictimPrestigeLevel,
                               SUM( kills.Points ) Points,
                               SUM( kills.Points ) NetScore,
                               SUM( 1 ) Kills,
                               SUM( CASE WHEN kills.IsHeadshot = 1
                                           THEN 1
                                         ELSE 0 END ) HeadshotKills,
                               SUM( CASE WHEN kills.Points <> 0
                                           THEN 1
                                         ELSE 0 END ) ScoredKills,
                               SUM( CASE WHEN kills.Points = 0
                                           THEN 1
                                         ELSE 0 END ) ZeroPointKills,
                               SUM( CASE WHEN ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 )
                                           THEN 1
                                         ELSE 0 END ) AssistedKills,
                               SUM( CASE WHEN damage_sums.TotalDamages > 0
                                           THEN 1
                                         ELSE 0 END ) DamageAssistedKills,
                               SUM( CASE WHEN damage_sums.TotalDamages > 0 AND (grenade_sums.TotalGrenades IS NULL OR grenade_sums.TotalGrenades = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0)
                                           THEN 1
                                         ELSE 0 END ) DamageAssistedOnlyKills,
                               SUM( CASE WHEN grenade_sums.TotalGrenades > 0
                                           THEN 1
                                         ELSE 0 END ) GrenadeAssistedKills,
                               SUM( CASE WHEN grenade_sums.TotalGrenades > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (spot_sums.TotalSpots Is NULL OR spot_sums.TotalSpots = 0)
                                           THEN 1
                                         ELSE 0 END ) GrenadeAssistedOnlyKills,
                               SUM( CASE WHEN spot_sums.TotalSpots > 0
                                           THEN 1
                                         ELSE 0 END ) SpotAssistedKills,
                               SUM( CASE WHEN spot_sums.TotalSpots > 0 AND (damage_sums.TotalDamages IS NULL OR damage_sums.TotalDamages = 0) AND (grenade_sums.TotalGrenades Is NULL OR grenade_sums.TotalGrenades  = 0)
                                           THEN 1
                                         ELSE 0 END ) SpotAssistedOnlyKills,
                               SUM( CASE WHEN damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedKills,
                               SUM( CASE WHEN damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL AND kills.IsHeadshot = 1
                                           THEN 1
                                         ELSE 0 END ) UnassistedHeadshotKills
                          FROM [dbo].ScrimDeath kills
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, damages.AttackerTeamOrdinal, VictimCharacterId, damages.Timestamp, COUNT(*) TotalDamages
                                                FROM [dbo].ScrimDamageAssist damages
                                                GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId ) damage_sums
                              ON kills.ScrimMatchId = damage_sums.ScrimMatchId
                                 AND kills.Timestamp = damage_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = damage_sums.AttackerTeamOrdinal
                                 AND kills.VictimCharacterId = damage_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, grenades.AttackerTeamOrdinal, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades
                                                FROM [dbo].[ScrimGrenadeAssist] grenades
                                                GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId ) grenade_sums
                                  ON kills.ScrimMatchId = grenade_sums.ScrimMatchId
                                     AND kills.Timestamp = grenade_sums.Timestamp
                                     AND kills.AttackerTeamOrdinal = grenade_sums.AttackerTeamOrdinal
                                     AND kills.VictimCharacterId = grenade_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, spots.SpotterTeamOrdinal, VictimCharacterId, spots.Timestamp, COUNT(*) TotalSpots
                                                FROM [dbo].[ScrimSpotAssist] spots
                                                GROUP BY ScrimMatchId, SpotterTeamOrdinal, Timestamp, VictimCharacterId ) spot_sums
                              ON kills.ScrimMatchId = spot_sums.ScrimMatchId
                                 AND kills.Timestamp = spot_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = spot_sums.SpotterTeamOrdinal
                                 AND kills.VictimCharacterId = spot_sums.VictimCharacterId
                            LEFT OUTER JOIN [dbo].ScrimMatchParticipatingPlayer victim_players
                              ON victim_players.ScrimMatchId = kills.ScrimMatchId
                                  AND victim_players.CharacterId = kills.VictimCharacterId
                          GROUP BY kills.ScrimMatchId, kills.AttackerCharacterId, kills.VictimCharacterId ) kill_sums
        ON match_players.ScrimMatchId = kill_sums.ScrimMatchId
           AND match_players.TeamOrdinal = kill_sums.TeamOrdinal
           AND match_players.CharacterId = kill_sums.CharacterId
           AND match_players2.CharacterId = kill_sums.KillVictimCharacterId
  WHERE kill_sums.Kills <> 0
      OR death_sums.Deaths <> 0
      OR damages_dealt.TotalDamageAssistsDealt <> 0
      OR damages_taken.TotalDamageAssistsTaken <> 0