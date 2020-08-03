using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Models.ScrimMatchReports
{
    public class ScrimMatchReportInfantryPlayerStats
    {
        public string ScrimMatchId { get; set; }
        public string CharacterId { get; set; }
        public int TeamOrdinal { get; set; }
        public string NameDisplay { get; set; }
        public string NameFull { get; set; }
        public int FactionId { get; set; }
        public int WorldId { get; set; }
        public int PrestigeLevel { get; set; }
        //public bool IsFromOutfit { get; set; }
        //public string OutfitId { get; set; }
        //public string OutfitTag { get; set; }
        //public bool IsFromConstructedTeam { get; set; }
        //public int? ConstructedTeamId { get; set; }
        public int Points { get; set; }
        public int NetScore { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int TeamKills { get; set; }
        public int Suicides { get; set; }
        public int ScoredDeaths { get; set; }
        public int ZeroPointDeaths { get; set; }
        public int TeamKillDeaths { get; set; }
        public int DamageAssists { get; set; }
        public int DamageTeamAssists { get; set; }
        public int DamageAssistedKills { get; set; }
        public int DamageAssistedDeaths { get; set; }
        public int DamageAssistedEnemyDeaths { get; set; }
        public int UnassistedEnemyDeaths { get; set; }
        public int KillsAsHeavyAssault { get; set; }
        public int KillsAsInfiltrator { get; set; }
        public int KillsAsLightAssault { get; set; }
        public int KillsAsMedic { get; set; }
        public int KillsAsEngineer { get; set; }
        public int KillsAsMax { get; set; }
        public int DeathsAsHeavyAssault { get; set; }
        public int DeathsAsInfiltrator { get; set; }
        public int DeathsAsLightAssault { get; set; }
        public int DeathsAsMedic { get; set; }
        public int DeathsAsEngineer { get; set; }
        public int DeathsAsMax { get; set; }

        public int OneVsOneCount => UnassistedEnemyDeaths + UnassistedKills;

        public int UnassistedKills => (Kills - DamageAssistedKills);

        public double OneVsOneRatio
        {
            get
            {
                if (OneVsOneCount > 0)
                {
                    return Math.Round((double)(UnassistedKills / (double)UnassistedEnemyDeaths), 2);
                }
                else
                {
                    return UnassistedKills / 1.0;
                }
            }
        }
    }
}
