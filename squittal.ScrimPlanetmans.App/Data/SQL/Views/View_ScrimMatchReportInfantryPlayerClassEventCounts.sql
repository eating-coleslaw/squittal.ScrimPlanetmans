USE [PlanetmansDbContext];

IF (NOT EXISTS (SELECT 1 FROM sys.views WHERE name = 'View_ScrimMatchReportInfantryPlayerClassEventCounts'))
BEGIN
    EXECUTE('CREATE VIEW View_ScrimMatchReportInfantryPlayerClassEventCounts as SELECT 1 as x');
END;

GO

ALTER VIEW View_ScrimMatchReportInfantryPlayerClassEventCounts AS
-- CREATE OR ALTER VIEW View_ScrimMatchReportInfantryPlayerClassEventCounts AS

  SELECT match_players.ScrimMatchId,
         match_players.TeamOrdinal,
         match_players.CharacterId,
         match_players.NameDisplay,
         match_players.NameFull,
         match_players.FactionId,
         match_players.WorldId,
         match_players.PrestigeLevel,
         COALESCE( kill_sums.KillsAsHeavyAssault, 0 ) + COALESCE( death_sums.DeathsAsHeavyAssault, 0 ) + COALESCE( damage_sums.DamageAssistsAsHeavyAssault, 0 ) EventsAsHeavyAssault,
         COALESCE( kill_sums.KillsAsInfiltrator, 0 ) + COALESCE( death_sums.DeathsAsInfiltrator, 0 ) + COALESCE( damage_sums.DamageAssistsAsInfiltrator, 0 ) EventsAsInfiltrator,
         COALESCE( kill_sums.KillsAsLightAssault, 0 ) + COALESCE( death_sums.DeathsAsLightAssault, 0 ) + COALESCE( damage_sums.DamageAssistsAsLightAssault, 0 ) EventsAsLightAssault,
         COALESCE( kill_sums.KillsAsMedic, 0 ) + COALESCE( death_sums.DeathsAsMedic, 0 ) + COALESCE( damage_sums.DamageAssistsAsMedic, 0 ) EventsAsMedic,
         COALESCE( kill_sums.KillsAsEngineer, 0 ) + COALESCE( death_sums.DeathsAsEngineer, 0 ) + COALESCE( damage_sums.DamageAssistsAsEngineer, 0 ) EventsAsEngineer,
         COALESCE( kill_sums.KillsAsMax, 0 ) + COALESCE( death_sums.DeathsAsMax, 0 ) + COALESCE( damage_sums.DamageAssistsAsMax, 0 ) EventsAsMax,
         COALESCE( kill_sums.KillsAsHeavyAssault, 0 ) KillsAsHeavyAssault,
         COALESCE( kill_sums.KillsAsInfiltrator, 0 ) KillsAsInfiltrator,
         COALESCE( kill_sums.KillsAsLightAssault, 0 ) KillsAsLightAssault,
         COALESCE( kill_sums.KillsAsMedic, 0 ) KillsAsMedic,
         COALESCE( kill_sums.KillsAsEngineer, 0 ) KillsAsEngineer,
         COALESCE( kill_sums.KillsAsMax, 0 ) KillsAsMax,
         COALESCE( death_sums.DeathsAsHeavyAssault, 0 ) DeathsAsHeavyAssault,
         COALESCE( death_sums.DeathsAsInfiltrator, 0 ) DeathsAsInfiltrator,
         COALESCE( death_sums.DeathsAsLightAssault, 0 ) DeathsAsLightAssault,
         COALESCE( death_sums.DeathsAsMedic, 0 ) DeathsAsMedic,
         COALESCE( death_sums.DeathsAsEngineer, 0 ) DeathsAsEngineer,
         COALESCE( death_sums.DeathsAsMax, 0 ) DeathsAsMax,
         COALESCE( damage_sums.DamageAssistsAsHeavyAssault, 0 ) DamageAssistsAsHeavyAssault,
         COALESCE( damage_sums.DamageAssistsAsInfiltrator, 0 ) DamageAssistsAsInfiltrator,
         COALESCE( damage_sums.DamageAssistsAsLightAssault, 0 ) DamageAssistsAsLightAssault,
         COALESCE( damage_sums.DamageAssistsAsMedic, 0 ) DamageAssistsAsMedic,
         COALESCE( damage_sums.DamageAssistsAsEngineer, 0 ) DamageAssistsAsEngineer,
         COALESCE( damage_sums.DamageAssistsAsMax, 0 ) DamageAssistsAsMax
    FROM [dbo].ScrimMatchParticipatingPlayer match_players
      LEFT OUTER JOIN ( SELECT ScrimMatchId,
                               AttackerCharacterId CharacterId,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 1, 8, 15) THEN 1 ELSE 0 END )  KillsAsInfiltrator,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 3, 10, 17) THEN 1 ELSE 0 END ) KillsAsLightAssault,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 4, 11, 18) THEN 1 ELSE 0 END ) KillsAsMedic,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 5, 12, 19) THEN 1 ELSE 0 END ) KillsAsEngineer,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 6, 13, 20) THEN 1 ELSE 0 END ) KillsAsHeavyAssault,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 7, 14, 21) THEN 1 ELSE 0 END ) KillsAsMax
                          FROM [dbo].ScrimDeath kills
                          GROUP BY ScrimMatchId, AttackerCharacterId ) kill_sums
        ON match_players.ScrimMatchId = kill_sums.ScrimMatchId
            AND match_players.CharacterId = kill_sums.CharacterId
      LEFT OUTER JOIN ( SELECT ScrimMatchId,
                               VictimCharacterId CharacterId,
                               SUM( CASE WHEN VictimLoadoutId IN ( 1, 8, 15) THEN 1 ELSE 0 END )  DeathsAsInfiltrator,
                               SUM( CASE WHEN VictimLoadoutId IN ( 3, 10, 17) THEN 1 ELSE 0 END ) DeathsAsLightAssault,
                               SUM( CASE WHEN VictimLoadoutId IN ( 4, 11, 18) THEN 1 ELSE 0 END ) DeathsAsMedic,
                               SUM( CASE WHEN VictimLoadoutId IN ( 5, 12, 19) THEN 1 ELSE 0 END ) DeathsAsEngineer,
                               SUM( CASE WHEN VictimLoadoutId IN ( 6, 13, 20) THEN 1 ELSE 0 END ) DeathsAsHeavyAssault,
                               SUM( CASE WHEN VictimLoadoutId IN ( 7, 14, 21) THEN 1 ELSE 0 END ) DeathsAsMax
                          FROM [dbo].ScrimDeath deaths
                          GROUP BY ScrimMatchId, VictimCharacterId ) death_sums
        ON match_players.ScrimMatchId = death_sums.ScrimMatchId
          AND match_players.CharacterId = death_sums.CharacterId
      LEFT OUTER JOIN ( SELECT ScrimMatchId,
                               AttackerCharacterId,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 1, 8, 15) THEN 1 ELSE 0 END )  DamageAssistsAsInfiltrator,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 3, 10, 17) THEN 1 ELSE 0 END ) DamageAssistsAsLightAssault,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 4, 11, 18) THEN 1 ELSE 0 END ) DamageAssistsAsMedic,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 5, 12, 19) THEN 1 ELSE 0 END ) DamageAssistsAsEngineer,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 6, 13, 20) THEN 1 ELSE 0 END ) DamageAssistsAsHeavyAssault,
                               SUM( CASE WHEN AttackerLoadoutId IN ( 7, 14, 21) THEN 1 ELSE 0 END ) DamageAssistsAsMax
                          FROM [dbo].ScrimDamageAssist damages
                          GROUP BY ScrimMatchId, AttackerCharacterId ) damage_sums
        ON match_players.ScrimMatchId = damage_sums.ScrimMatchId
            AND match_players.CharacterId = damage_sums.AttackerCharacterId