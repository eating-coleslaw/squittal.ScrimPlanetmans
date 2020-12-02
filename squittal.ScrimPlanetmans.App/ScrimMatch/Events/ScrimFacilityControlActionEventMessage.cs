using squittal.ScrimPlanetmans.Models.MessageLogs;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Messages
{
    public class ScrimFacilityControlActionEventMessage : ScrimActionEventMessage
    {
        public ScrimFacilityControlActionEvent FacilityControl { get; set; }

        public ScrimFacilityControlActionEventMessage(ScrimFacilityControlActionEvent facilityControl)
        {
            FacilityControl = facilityControl;

            Info = GetInfo();

            Timestamp = facilityControl.Timestamp;

            LogLevel = facilityControl.IsBanned ? ScrimMessageLogLevel.MatchEventRuleBreak : ScrimMessageLogLevel.MatchEventMajor;
        }

        private string GetInfo()
        {
            var teamOrdinal = FacilityControl.ControllingTeamOrdinal;

            var actionDisplay = GetEnumValueName(FacilityControl.ActionType);
            var controlTypeDisplay = Enum.GetName(typeof(FacilityControlType), FacilityControl.ControlType).ToUpper();

            var facilityName = FacilityControl.FacilityName;
            var facilityId = FacilityControl.FacilityId;

            var pointsDisplay = GetPointsDisplay(FacilityControl.Points);

            var bannedDisplay = FacilityControl.IsBanned ? "RULE BREAK - " : string.Empty;

            return $"{bannedDisplay}Team {teamOrdinal} {actionDisplay} {controlTypeDisplay}: {pointsDisplay} {facilityName} [{facilityId}]";
        }
    }
}
