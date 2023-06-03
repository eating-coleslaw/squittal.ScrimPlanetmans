USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchReportInfantryPlayerWeaponStats'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchReportInfantryPlayerWeaponStats as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchReportInfantryPlayerWeaponStats AS
-- CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerWeaponStats AS

  SELECT player_match_weapons.ScrimMatchId,
        player_match_weapons.TeamOrdinal TeamOrdinal,
        player_match_weapons.CharacterId CharacterId,
        player_match_weapons.NameDisplay NameDisplay,
        player_match_weapons.NameFull NameFull,
        player_match_weapons.FactionId FactionId,
        player_match_weapons.PrestigeLevel PrestigeLevel,
        player_match_weapons.WeaponId WeaponId,
        player_match_weapons.WeaponName WeaponName,
        player_match_weapons.WeaponFactionId WeaponFactionId,
        COALESCE(player_match_weapons.Points, 0) Points,
        COALESCE(player_match_weapons.Kills, 0) Kills,
        COALESCE(player_match_weapons.HeadshotKills, 0) HeadshotKills,
        COALESCE(player_match_weapons.Teamkills, 0) Teamkills,
        COALESCE(player_match_weapons.ScoredKills, 0) ScoredKills,
        COALESCE(player_match_weapons.ZeroPointKills, 0) ZeroPointKills,
        COALESCE(player_match_weapons.TeamkillDeaths, 0) TeamkillDeaths,
        COALESCE(player_match_weapons.Suicides, 0) Suicides,
        COALESCE(player_match_weapons.Deaths, 0) Deaths,
        COALESCE(player_match_weapons.HeadshotDeaths, 0) HeadshotDeaths,
        COALESCE(player_match_weapons.DamageAssistedKills, 0) DamageAssistedKills,
        COALESCE(player_match_weapons.GrenadeAssistedKills, 0) GrenadeAssistedKills,
        COALESCE(player_match_weapons.AssistedKills, 0) AssistedKills,
        COALESCE(player_match_weapons.UnassistedKills, 0) UnassistedKills,
        COALESCE(player_match_weapons.UnassistedHeadshotKills, 0) UnassistedHeadshotKills,
        COALESCE(player_match_weapons.HeadshotTeamkills, 0) HeadshotTeamkills,
        COALESCE(player_match_weapons.DamageAssistedDeaths, 0) DamageAssistedDeaths,
        COALESCE(player_match_weapons.GrenadeAssistedDeaths, 0) GrenadeAssistedDeaths,
        COALESCE(player_match_weapons.AssistedDeaths, 0) AssistedDeaths,
        COALESCE(player_match_weapons.UnassistedDeaths, 0) UnassistedDeaths,
        COALESCE(player_match_weapons.UnassistedHeadshotDeaths, 0) UnassistedHeadshotDeaths,
        COALESCE(player_match_weapons.EnemyDeaths, 0) EnemyDeaths,
        COALESCE(player_match_weapons.HeadshotEnemyDeaths, 0) HeadshotEnemyDeaths,
        COALESCE(player_match_weapons.ScoredDeaths, 0) ScoredDeaths,
        COALESCE(player_match_weapons.ZeroPointDeaths, 0) ZeroPointDeaths,
        COALESCE(player_match_weapons.DamageAssistedEnemyDeaths, 0) DamageAssistedEnemyDeaths,
        COALESCE(player_match_weapons.GrenadeAssistedEnemyDeaths, 0) GrenadeAssistedEnemyDeaths,
        COALESCE(player_match_weapons.AssistedEnemyDeaths, 0) AssistedEnemyDeaths,
        COALESCE(player_match_weapons.UnassistedEnemyDeaths, 0) UnassistedEnemyDeaths,
        COALESCE(player_match_weapons.UnassistedHeadshotEnemyDeaths, 0) UnassistedHeadshotEnemyDeaths
    FROM ( SELECT match_players.ScrimMatchId,
                  match_players.CharacterId CharacterId,
                  weapons.Id WeaponId,
                  MAX( match_players.TeamOrdinal ) TeamOrdinal,
                  MAX( match_players.NameDisplay ) NameDisplay,
                  MAX( match_players.NameFull ) NameFull,
                  MAX( match_players.FactionId ) FactionId,
                  MAX( match_players.PrestigeLevel ) PrestigeLevel,
                  MAX( weapons.Name ) WeaponName,
                  MAX( CASE WHEN COALESCE( weapons.FactionId, 0 ) = 0 THEN 4 ELSE weapons.FactionId END ) WeaponFactionId,
        
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId THEN deaths.Points
                            ELSE 0 END ) Points,
        
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 THEN 1
                            ELSE 0 END ) Kills,
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND deaths.Points <> 0 THEN 1
                            ELSE 0 END ) ScoredKills, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND deaths.Points = 0 THEN 1
                            ELSE 0 END ) ZeroPointKills, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 1 THEN 1
                            ELSE 0 END ) Teamkills, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND deaths.IsHeadshot = 1 THEN 1
                            ELSE 0 END ) HeadshotKills, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 1 AND deaths.IsHeadshot = 1 THEN 1
                            ELSE 0 END ) HeadshotTeamkills, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 ) THEN 1
                            ELSE 0 END ) AssistedKills,
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1
                            ELSE 0 END ) DamageAssistedKills,
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1
                            ELSE 0 END ) GrenadeAssistedKills,
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1
                            ELSE 0 END ) UnassistedKills,
                  SUM( CASE WHEN match_players.CharacterId = deaths.AttackerCharacterId AND deaths.DeathType = 0 AND deaths.IsHeadshot = 1 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1
                            ELSE 0 END ) UnassistedHeadshotKills,
        
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId THEN 1
                            ELSE 0 END ) Deaths, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 AND deaths.Points <> 0 THEN 1
                            ELSE 0 END ) ScoredDeaths, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.Points = 0 THEN 1
                            ELSE 0 END ) ZeroPointDeaths, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 AND deaths.IsHeadshot = 1 THEN 1
                            ELSE 0 END ) HeadshotEnemyDeaths, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.IsHeadshot = 1 THEN 1
                            ELSE 0 END ) HeadshotDeaths, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 THEN 1
                            ELSE 0 END ) EnemyDeaths, 
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 1 THEN 1
                            ELSE 0 END ) TeamkillDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 2 THEN 1
                            ELSE 0 END ) Suicides,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 ) THEN 1
                            ELSE 0 END ) AssistedDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND damage_sums.TotalDamages > 0 THEN 1
                            ELSE 0 END ) DamageAssistedDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND grenade_sums.TotalGrenades > 0 THEN 1
                            ELSE 0 END ) GrenadeAssistedDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1
                            ELSE 0 END ) UnassistedDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.IsHeadshot = 1 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1
                            ELSE 0 END ) UnassistedHeadshotDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 AND ( damage_sums.TotalDamages > 0 OR grenade_sums.TotalGrenades > 0 ) THEN 1
                            ELSE 0 END ) AssistedEnemyDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 AND damage_sums.TotalDamages > 0 THEN 1
                            ELSE 0 END ) DamageAssistedEnemyDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 AND grenade_sums.TotalGrenades > 0 THEN 1
                            ELSE 0 END ) GrenadeAssistedEnemyDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1
                            ELSE 0 END ) UnassistedEnemyDeaths,
                  SUM( CASE WHEN match_players.CharacterId = deaths.VictimCharacterId AND deaths.DeathType = 0 AND deaths.IsHeadshot = 1 AND damage_sums.TotalDamages IS NULL AND grenade_sums.TotalGrenades IS NULL THEN 1
                            ELSE 0 END ) UnassistedHeadshotEnemyDeaths
              FROM  [dbo].ScrimMatchParticipatingPlayer match_players
                INNER JOIN [dbo].ScrimDeath deaths
                  ON match_players.ScrimMatchId = deaths.ScrimMatchId
                      AND ( match_players.CharacterId = deaths.AttackerCharacterId
                            OR match_players.CharacterId = deaths.VictimCharacterId ) 
                LEFT OUTER JOIN ( SELECT ScrimMatchId, VictimCharacterId, damages.Timestamp, COUNT(*) TotalDamages
                                    FROM [dbo].ScrimDamageAssist damages
                                    GROUP BY ScrimMatchId, Timestamp, VictimCharacterId ) damage_sums
                  ON deaths.ScrimMatchId = damage_sums.ScrimMatchId
                      AND deaths.Timestamp = damage_sums.Timestamp
                      AND deaths.VictimCharacterId = damage_sums.VictimCharacterId
                LEFT OUTER JOIN ( SELECT ScrimMatchId, VictimCharacterId, grenades.Timestamp, COUNT(*) TotalGrenades
                                    FROM [dbo].[ScrimGrenadeAssist] grenades
                                    GROUP BY ScrimMatchId, Timestamp, VictimCharacterId ) grenade_sums
                  ON deaths.ScrimMatchId = grenade_sums.ScrimMatchId
                      AND deaths.Timestamp = grenade_sums.Timestamp
                      AND deaths.VictimCharacterId = grenade_sums.VictimCharacterId
                LEFT OUTER JOIN Item weapons
                  ON deaths.WeaponId = weapons.Id
              WHERE DeathType IN (0, 1) AND WeaponId IS NOT NULL AND WeaponId <> 0
              GROUP BY match_players.ScrimMatchId, match_players.CharacterId, weapons.Id ) player_match_weapons
    WHERE ( player_match_weapons.Kills <> 0
            OR player_match_weapons.Deaths <> 0 )