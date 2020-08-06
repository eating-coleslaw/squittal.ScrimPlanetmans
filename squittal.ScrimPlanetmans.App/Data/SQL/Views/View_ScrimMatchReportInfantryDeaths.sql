USE [PlanetmansDbContext];

GO

CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerDeaths AS

  SELECT deaths.ScrimMatchId,
       deaths.Timestamp,
       deaths.AttackerCharacterId,
       deaths.VictimCharacterId,
       MAX(deaths.ScrimMatchRound) ScrimMatchRound,
       MAX(deaths.Points) Points,
       MAX(deaths.ActionType) ActionType,
       MAX(deaths.DeathType) DeathType,
       MAX(deaths.AttackerTeamOrdinal) AttackerTeamOrdinal,
       MAX(deaths.VictimTeamOrdinal) VictimTeamOrdinal,
       MAX(CASE WHEN deaths.AttackerCharacterId = match_players.CharacterId THEN match_players.NameDisplay ELSE NULL END) AttackerNameDisplay,
       MAX(CASE WHEN deaths.VictimCharacterId = match_players.CharacterId THEN match_players.NameDisplay ELSE NULL END) VictimNameDisplay,
       MAX(deaths.AttackerNameFull) AttackerNameFull,
       MAX(deaths.VictimNameFull) VictimNameFull,
       MAX(deaths.AttackerFactionId) AttackerFactionId,
       MAX(deaths.VictimFactionId) VictimFactionId,
       MAX(deaths.AttackerLoadoutId) AttackerLoadoutId,
       MAX(deaths.VictimLoadoutId) VictimLoadoutId,
       MAX(CASE WHEN deaths.IsHeadshot = 1 THEN 1 ELSE 0 END) IsHeadshot,
       MAX(deaths.WeaponId) WeaponId,
       MAX(deaths.ZoneId) ZoneId,
       MAX(deaths.WorldId) WorldId,
       MAX(damage_sums.DamageAssists) DamageAssists,
       MAX(grenade_sums.ConcussionGrenadeAssists ) ConcussionGrenadeAssists,
       MAX(grenade_sums.EmpGrenadeAssists) EmpGrenadeAssists,
       MAX(grenade_sums.FlashGrenadeAssists) FlashGrenadeAssists,
       MAX(spot_sums.SpotAssists ) SpotAssists
    FROM ScrimDeath deaths
      LEFT OUTER JOIN ( SELECT damages.ScrimMatchId,
                                damages.Timestamp,
                                damages.VictimCharacterId,
                                SUM( CASE WHEN damages.ActionType = 304 THEN 1 ELSE 0 END ) DamageAssists,
                                SUM( CASE WHEN damages.ActionType = 310 THEN 1 ELSE 0 END ) DamageTeamAssists,
                                SUM( CASE WHEN damages.ActionType = 312 THEN 1 ELSE 0 END ) DamageSelfAssists
                          FROM ScrimDamageAssist damages
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
                          FROM ScrimGrenadeAssist grenades
                          GROUP BY grenades.ScrimMatchId, grenades.Timestamp, grenades.VictimCharacterId ) grenade_sums
                ON grenade_sums.ScrimMatchId = deaths.ScrimMatchId
                    AND grenade_sums.Timestamp = deaths.Timestamp
                    AND grenade_sums.VictimCharacterId = deaths.VictimCharacterId
      LEFT OUTER JOIN ( SELECT spots.ScrimMatchId,
                                spots.Timestamp,
                                spots.VictimCharacterId,
                                SUM( CASE WHEN spots.ActionType = 308 THEN 1 ELSE 0 END ) SpotAssists
                          FROM ScrimSpotAssist spots
                          GROUP BY spots.ScrimMatchId, spots.Timestamp, spots.VictimCharacterId ) spot_sums
                ON grenade_sums.ScrimMatchId = deaths.ScrimMatchId
                    AND grenade_sums.Timestamp = deaths.Timestamp
                    AND grenade_sums.VictimCharacterId = deaths.VictimCharacterId
      LEFT OUTER JOIN ScrimMatchParticipatingPlayer match_players
        ON match_players.ScrimMatchId = deaths.ScrimMatchId
            AND ( match_players.CharacterId = deaths.AttackerCharacterId
                  OR match_players.CharacterId = deaths.VictimCharacterId )
      GROUP BY deaths.ScrimMatchId, deaths.Timestamp, deaths.AttackerCharacterId, deaths.VictimCharacterId