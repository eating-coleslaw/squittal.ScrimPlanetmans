USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchReportInfantryPlayerWeaponStats'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchReportInfantryPlayerWeaponStats as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchReportInfantryPlayerWeaponStats AS
-- CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerWeaponStats AS

  SELECT match_players.ScrimMatchId,
         match_players.TeamOrdinal TeamOrdinal,
         match_players.CharacterId CharacterId,
         match_players.NameDisplay NameDisplay,
         match_players.NameFull NameFull,
         match_players.FactionId FactionId,
         match_players.PrestigeLevel PrestigeLevel,
         player_weapon_combos.WeaponId WeaponId,
         weapons.Name WeaponName,
         COALESCE( weapons.FactionId, 4 ) WeaponFactionId,
         COALESCE(kill_sums.Points, 0) Points,
         COALESCE(kill_sums.NetScore, 0) + COALESCE(death_sums.NetScore, 0) NetScore,
         COALESCE(kill_sums.Kills, 0) Kills,
         COALESCE(kill_sums.HeadshotKills, 0) HeadshotKills,
         COALESCE(kill_sums.Teamkills, 0) Teamkills,
         COALESCE(kill_sums.ScoredKills, 0) ScoredKills,
         COALESCE(kill_sums.ZeroPointKills, 0) ZeroPointKills,
         COALESCE(death_sums.TeamkillDeaths, 0) TeamkillDeaths,
         COALESCE(death_sums.Suicides, 0) Suicides,
         COALESCE(death_sums.Deaths, 0) Deaths,
         COALESCE(death_sums.HeadshotDeaths, 0) HeadshotDeaths,
         COALESCE(kill_sums.DamageAssistedKills, 0) DamageAssistedKills,
         COALESCE(kill_sums.GrenadeAssistedKills, 0) GrenadeAssistedKills,
         COALESCE(kill_sums.AssistedKills, 0) AssistedKills,
         COALESCE(kill_sums.UnassistedKills, 0) UnassistedKills,
         COALESCE(kill_sums.UnassistedHeadshotKills, 0) UnassistedHeadshotKills,
         COALESCE(kill_sums.HeadshotTeamkills, 0) HeadshotTeamkills,
         COALESCE(death_sums.DamageAssistedDeaths, 0) DamageAssistedDeaths,
         COALESCE(death_sums.GrenadeAssistedDeaths, 0) GrenadeAssistedDeaths,
         COALESCE(death_sums.AssistedDeaths, 0) AssistedDeaths,
         COALESCE(death_sums.UnassistedDeaths, 0) UnassistedDeaths,
         COALESCE(death_sums.UnassistedHeadshotDeaths, 0) UnassistedHeadshotDeaths,
         COALESCE(death_sums.EnemyDeaths, 0) EnemyDeaths,
         COALESCE(death_sums.HeadshotEnemyDeaths, 0) HeadshotEnemyDeaths,
         COALESCE(death_sums.ScoredDeaths, 0) ScoredDeaths,
         COALESCE(death_sums.ZeroPointDeaths, 0) ZeroPointDeaths,
         COALESCE(death_sums.ScoredEnemyDeaths, 0) ScoredEnemyDeaths,
         COALESCE(death_sums.ZeroPointEnemyDeaths, 0) ZeroPointEnemyDeaths,
         COALESCE(death_sums.DamageAssistedEnemyDeaths, 0) DamageAssistedEnemyDeaths,
         COALESCE(death_sums.GrenadeAssistedEnemyDeaths, 0) GrenadeAssistedEnemyDeaths,
         COALESCE(death_sums.AssistedEnemyDeaths, 0) AssistedEnemyDeaths,
         COALESCE(death_sums.UnassistedEnemyDeaths, 0) UnassistedEnemyDeaths,
         COALESCE(death_sums.UnassistedHeadshotEnemyDeaths, 0) UnassistedHeadshotEnemyDeaths
    FROM [PlanetmansDbContext].[dbo].ScrimMatchParticipatingPlayer match_players
      INNER JOIN ( SELECT VictimCharacterId CharacterId, WeaponId
                     FROM [PlanetmansDbContext].[dbo].ScrimDeath deaths
                     WHERE DeathType IN (0, 1) AND WeaponId IS NOT NULL AND WeaponId <> 0
                     GROUP BY VictimCharacterId, WeaponId
                     
                   UNION

                   SELECT AttackerCharacterId CharacterId, WeaponId
                     FROM [PlanetmansDbContext].[dbo].ScrimDeath deaths
                     WHERE DeathType IN (0, 1) AND WeaponId IS NOT NULL AND WeaponId <> 0
                     GROUP BY AttackerCharacterId, WeaponId ) player_weapon_combos
        ON match_players.CharacterId = player_weapon_combos.CharacterId
      LEFT OUTER JOIN ( SELECT deaths.ScrimMatchId,
                               deaths.VictimCharacterId CharacterId,
                               deaths.WeaponId,
                               SUM( CASE WHEN DeathType = 0
                                           THEN deaths.Points * -1
                                         ELSE 0 END ) NetScore,
                               SUM( 1 ) Deaths,
                               SUM( CASE WHEN DeathType = 0
                                           THEN 1
                                         ELSE 0 END ) EnemyDeaths,
                               SUM( CASE WHEN DeathType = 1
                                           THEN 1
                                         ELSE 0 END ) TeamkillDeaths,
                               SUM( CASE WHEN DeathType = 2
                                           THEN 1
                                         ELSE 0 END ) Suicides,
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
                               SUM( CASE WHEN grenade_sums.TotalGrenades > 0
                                           THEN 1
                                         ELSE 0 END ) GrenadeAssistedDeaths,
                               SUM( CASE WHEN damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedDeaths,

                               SUM( CASE WHEN deaths.IsHeadshot = 1 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedHeadshotDeaths,
                               SUM( CASE WHEN DeathType = 0 AND deaths.IsHeadshot = 1
                                           THEN 1
                                         ELSE 0 END ) HeadshotEnemyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND deaths.Points <> 0
                                           THEN 1
                                         ELSE 0 END ) ScoredEnemyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND deaths.Points = 0
                                           THEN 1
                                         ELSE 0 END ) ZeroPointEnemyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 )
                                           THEN 1
                                         ELSE 0 END ) AssistedEnemyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages > 0
                                           THEN 1
                                         ELSE 0 END ) DamageAssistedEnemyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND grenade_sums.TotalGrenades > 0
                                           THEN 1
                                         ELSE 0 END ) GrenadeAssistedEnemyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedEnemyDeaths,
                               SUM( CASE WHEN DeathType = 0 AND deaths.IsHeadshot = 1 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedHeadshotEnemyDeaths
                          FROM [PlanetmansDbContext].[dbo].ScrimDeath deaths
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, damages.VictimTeamOrdinal, VictimCharacterId, damages.Timestamp, COUNT(*) TotalDamages
                                                FROM [PlanetmansDbContext].[dbo].ScrimDamageAssist damages
                                                GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) damage_sums
                              ON deaths.ScrimMatchId = damage_sums.ScrimMatchId
                                 AND deaths.Timestamp = damage_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = damage_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = damage_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, grenades.VictimTeamOrdinal, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades
                                                FROM [PlanetmansDbContext].[dbo].[ScrimGrenadeAssist] grenades
                                                GROUP BY ScrimMatchId, VictimTeamOrdinal, Timestamp, VictimCharacterId ) grenade_sums
                              ON deaths.ScrimMatchId = grenade_sums.ScrimMatchId
                                 AND deaths.Timestamp = grenade_sums.Timestamp
                                 AND deaths.VictimTeamOrdinal = grenade_sums.VictimTeamOrdinal
                                 AND deaths.VictimCharacterId = grenade_sums.VictimCharacterId
                        WHERE DeathType IN (0, 1) AND WeaponId IS NOT NULL AND WeaponId <> 0
                        GROUP BY deaths.ScrimMatchId, deaths.VictimCharacterId, deaths.WeaponId ) death_sums
        ON match_players.ScrimMatchId = death_sums.ScrimMatchId
           AND match_players.CharacterId = death_sums.CharacterId
           AND player_weapon_combos.WeaponId = death_sums.WeaponId
      LEFT OUTER JOIN ( SELECT kills.ScrimMatchId,
                               kills.AttackerCharacterId CharacterId,
                               kills.WeaponId,
                               SUM( kills.Points ) Points,
                               SUM( kills.Points ) NetScore,
                               SUM( CASE WHEN DeathType = 0 THEN 1 ELSE 0 END ) Kills,
                               SUM( CASE WHEN DeathType = 0 AND kills.IsHeadshot = 1
                                           THEN 1
                                         ELSE 0 END ) HeadshotKills,
                               SUM( CASE WHEN kills.Points <> 0
                                           THEN 1
                                         ELSE 0 END ) ScoredKills,
                               SUM( CASE WHEN kills.Points = 0
                                           THEN 1
                                         ELSE 0 END ) ZeroPointKills,
                               SUM( CASE WHEN DeathType = 0 AND ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 )
                                           THEN 1
                                         ELSE 0 END ) AssistedKills,
                               SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages > 0
                                           THEN 1
                                         ELSE 0 END ) DamageAssistedKills,
                               SUM( CASE WHEN DeathType = 0 AND grenade_sums.TotalGrenades > 0
                                           THEN 1
                                         ELSE 0 END ) GrenadeAssistedKills,
                               SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL
                                           THEN 1
                                         ELSE 0 END ) UnassistedKills,
                               SUM( CASE WHEN DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL AND kills.IsHeadshot = 1
                                           THEN 1
                                         ELSE 0 END ) UnassistedHeadshotKills,
                               SUM( CASE WHEN DeathType = 1
                                           THEN 1
                                         ELSE 0 END ) Teamkills,
                               SUM( CASE WHEN DeathType = 1 AND kills.IsHeadshot = 1
                                           THEN 1
                                         ELSE 0 END ) HeadshotTeamkills
                          FROM [PlanetmansDbContext].[dbo].ScrimDeath kills
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, damages.AttackerTeamOrdinal, VictimCharacterId, damages.Timestamp, COUNT(*) TotalDamages
                                                FROM [PlanetmansDbContext].[dbo].ScrimDamageAssist damages
                                                GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId ) damage_sums
                              ON kills.ScrimMatchId = damage_sums.ScrimMatchId
                                 AND kills.Timestamp = damage_sums.Timestamp
                                 AND kills.AttackerTeamOrdinal = damage_sums.AttackerTeamOrdinal
                                 AND kills.VictimCharacterId = damage_sums.VictimCharacterId
                            LEFT OUTER JOIN ( SELECT ScrimMatchId, grenades.AttackerTeamOrdinal, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades
                                                FROM [PlanetmansDbContext].[dbo].[ScrimGrenadeAssist] grenades
                                                GROUP BY ScrimMatchId, AttackerTeamOrdinal, Timestamp, VictimCharacterId ) grenade_sums
                                  ON kills.ScrimMatchId = grenade_sums.ScrimMatchId
                                     AND kills.Timestamp = grenade_sums.Timestamp
                                     AND kills.AttackerTeamOrdinal = grenade_sums.AttackerTeamOrdinal
                                     AND kills.VictimCharacterId = grenade_sums.VictimCharacterId
                          WHERE DeathType IN (0, 1) AND WeaponId IS NOT NULL AND WeaponId <> 0
                          GROUP BY kills.ScrimMatchId, kills.AttackerCharacterId, kills.WeaponId ) kill_sums
        ON match_players.ScrimMatchId = kill_sums.ScrimMatchId
           AND match_players.CharacterId = kill_sums.CharacterId
           and player_weapon_combos.WeaponId = kill_sums.WeaponId
        LEFT OUTER JOIN Item weapons
          ON player_weapon_combos.WeaponId = weapons.Id
  WHERE kill_sums.Kills <> 0
     OR death_sums.Deaths <> 0