using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Shared.Models.Planetside.Events;
using squittal.ScrimPlanetmans.Shared.Models;
using System;
using System.Linq;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimMatchScorer : IScrimMatchScorer
    {
        private readonly IScrimRulesetManager _rulesets;
        private readonly IScrimTeamsManager _teamsManager;
        private readonly ILogger<ScrimMatchEngine> _logger;

        private Ruleset _activeRuleset;

        public ScrimMatchScorer(IScrimRulesetManager rulesets, IScrimTeamsManager teamsManager, ILogger<ScrimMatchEngine> logger)
        {
            _rulesets = rulesets;
            _teamsManager = teamsManager;
            _logger = logger;

            //_activeRuleset = _rulesets.GetActiveRuleset();
        }

        #region Death Events
        public async Task SetActiveRuleset()
        {
            //_activeRuleset = await _rulesets.GetDefaultRuleset();
            _activeRuleset = await _rulesets.GetActiveRuleset();
        }

        public int ScoreDeathEvent(ScrimDeathActionEvent death)
        {
            switch (death.DeathType)
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

        private int ScoreKill(ScrimDeathActionEvent death)
        {
            int points;

            if (death.ActionType == ScrimActionType.InfantryKillInfantry)
            {
                var categoryId = death.Weapon.ItemCategoryId;
                points = _activeRuleset.ItemCategoryRules
                                            .Where(rule => rule.ItemCategoryId == categoryId)
                                            .Select(rule => rule.Points)
                                            .FirstOrDefault();
            }
            else
            {
                var actionType = death.ActionType;
                points = _activeRuleset.ActionRules
                                            .Where(rule => rule.ScrimActionType == actionType)
                                            .Select(rule => rule.Points)
                                            .FirstOrDefault();
            }

            var isHeadshot = (death.IsHeadshot ? 1 : 0);

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                Kills = 1,
                Headshots = isHeadshot
            };

            var victimUpdate = new ScrimEventAggregate()
            {
                NetScore = -points,
                Deaths = 1,
                HeadshotDeaths = isHeadshot
            };

            // Player Stats update automatically updates the appropriate team's stats
            _teamsManager.UpdatePlayerStats(death.AttackerPlayer.Id, attackerUpdate);
            _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return points;
        }

        private int ScoreSuicide(ScrimDeathActionEvent death)
        {
            var actionType = death.ActionType;
            var points = _activeRuleset.ActionRules
                                        .Where(rule => rule.ScrimActionType == actionType)
                                        .Select(rule => rule.Points)
                                        .FirstOrDefault();

            var victimUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                Deaths = 1,
                Suicides = 1
            };

            // Player Stats update automatically updates the appropriate team's stats
            _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return points;
        }

        private int ScoreTeamkill(ScrimDeathActionEvent death)
        {
            var actionType = death.ActionType;
            var points = _activeRuleset.ActionRules
                                        .Where(rule => rule.ScrimActionType == actionType)
                                        .Select(rule => rule.Points)
                                        .FirstOrDefault();

            var attackerUpdate = new ScrimEventAggregate()
            {
                Points = points,
                NetScore = points,
                Teamkills = 1
            };

            var victimUpdate = new ScrimEventAggregate()
            {
                Deaths = 1,
                TeamkillDeaths = 1
            };

            // Player Stats update automatically updates the appropriate team's stats
            _teamsManager.UpdatePlayerStats(death.AttackerPlayer.Id, attackerUpdate);
            _teamsManager.UpdatePlayerStats(death.VictimPlayer.Id, victimUpdate);

            return points;
        }

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

        #region Misc. Non-Scored Events
        public void HandlePlayerLogin(PlayerLogin login)
        {
            var characterId = login.CharacterId;
            _teamsManager.SetPlayerOnlineStatus(characterId, true);
        }

        public void HandlePlayerLogout(PlayerLogout login)
        {
            var characterId = login.CharacterId;
            _teamsManager.SetPlayerOnlineStatus(characterId, false);
        }
        #endregion Misc. Non-Scored Events
    }
}
