using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class MatchStateUpdateMessage
    {
        public MatchState MatchState { get; set; }
        public DateTime Timestamp { get; set; }
        public int CurrentRound { get; set; }
        public string MatchTitle { get; set; }
        public string Info { get; set; }

        public MatchStateUpdateMessage(MatchState matchState, int currentRound, DateTime timestamp, string matchTitle)
        {
            MatchState = matchState;
            Timestamp = timestamp;
            CurrentRound = currentRound;
            MatchTitle = matchTitle;

            Info = $"Match \"{MatchTitle}\" State Changed: {Enum.GetName(typeof(MatchState), matchState).ToUpper()} | Round {CurrentRound}";
        }
    }
}
