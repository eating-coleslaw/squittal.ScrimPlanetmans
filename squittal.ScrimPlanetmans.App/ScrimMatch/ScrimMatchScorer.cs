using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;
using squittal.ScrimPlanetmans.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimMatchScorer : IScrimMatchScorer
    {
        private readonly IScrimTeamsManager _teamsManager;
        private readonly ILogger<ScrimMatchEngine> _logger;

        public ScrimMatchScorer(IScrimTeamsManager teamsManager, ILogger<ScrimMatchEngine> logger)
        {
            _teamsManager = teamsManager;
            _logger = logger;
        }

        #region Death Events
        public int ScoreDeathEvent(Death death)
        {
            var attackerId = death.AttackerCharacterId;
            var victimId = death.CharacterId;

            bool onSameTeam = _teamsManager.DoPlayersShareTeam(attackerId, victimId, out int? attackerTeamOrdinal, out int? victimTeamOrdinal);

            death.AttackerTeamOrdinal = attackerTeamOrdinal;
            death.CharacterTeamOrdinal = victimTeamOrdinal;

            death.DeathEventType = onSameTeam ? DeathEventType.Suicide : death.DeathEventType;

            switch (death.DeathEventType)
            {
                case DeathEventType.Kill:
                    return ScoreKill(death);

                case DeathEventType.Suicide:
                    return ScoreSuicide(death);

                case DeathEventType.Teamkill:
                    return ScoreTeamkill(death);

                default:
                    return 0;
            }
        }

        private int ScoreKill(Death death)
        {
            int points = 2;
            int headshot = (death.IsHeadshot ? 1 : 0);

            // Attacker Points
            if (death.AttackerTeamOrdinal != null)
            {
                var attackerAggregate = new ScrimEventAggregate()
                {
                    Points = points,
                    NetScore = points,
                    Kills = 1,
                    Headshots = headshot
                };

                // Player Stats update automatically updates the appropriate team's stats
                _teamsManager.UpdatePlayerStats(death.AttackerCharacterId, attackerAggregate);
            }

            // Victim Points
            if (death.CharacterTeamOrdinal != null)
            {
                var victimAggregate = new ScrimEventAggregate()
                {
                    NetScore = -points,
                    Deaths = 1,
                    HeadshotDeaths = headshot
                };

                // Player Stats update automatically updates the appropriate team's stats
                _teamsManager.UpdatePlayerStats(death.CharacterId, victimAggregate);
            }

            return points;
        }

        private int ScoreSuicide(Death death)
        {
            int points = -3;
            int headshot = (death.IsHeadshot ? 1 : 0);

            // Victim Points
            if (death.CharacterTeamOrdinal != null)
            {
                var victimAggregate = new ScrimEventAggregate()
                {
                    Points = points,
                    NetScore = points,
                    Deaths = 1,
                    Suicides = 1,
                    HeadshotDeaths = headshot
                };

                // Player Stats update automatically updates the appropriate team's stats
                _teamsManager.UpdatePlayerStats(death.CharacterId, victimAggregate);
            }

            return points;
        }

        private int ScoreTeamkill(Death death)
        {
            int points = -3;
            //int headshot = (death.IsHeadshot ? 1 : 0);

            // Attacker Points
            if (death.AttackerTeamOrdinal != null)
            {
                var attackerAggregate = new ScrimEventAggregate()
                {
                    Points = points,
                    NetScore = points,
                    Teamkills = 1
                };

                // Player Stats update automatically updates the appropriate team's stats
                _teamsManager.UpdatePlayerStats(death.AttackerCharacterId, attackerAggregate);
            }

            // Victim Points
            if (death.CharacterTeamOrdinal != null)
            {
                var victimAggregate = new ScrimEventAggregate()
                {
                    Deaths = 1,
                    TeamkillDeaths = 1
                };

                // Player Stats update automatically updates the appropriate team's stats
                _teamsManager.UpdatePlayerStats(death.CharacterId, victimAggregate);
            }

            return points;
        }

        #endregion Death Events


        #region Experience Events
        public int ScoreGainExperienceEvent(GainExperience expGain)
        {
            throw new NotImplementedException();
        }

        #endregion Experience Events

        #region Objective Events
        public int ScoreFacilityControlEvent(FacilityControl control)
        {
            throw new NotImplementedException();
        }

        public int ScorePlayerFacilityCaptureEvent(PlayerFacilityCapture capture)
        {
            throw new NotImplementedException();
        }

        public int ScorePlayerFacilityDefendEvent(PlayerFacilityDefend defense)
        {
            throw new NotImplementedException();
        }

        #endregion Objective Events
    }
}
