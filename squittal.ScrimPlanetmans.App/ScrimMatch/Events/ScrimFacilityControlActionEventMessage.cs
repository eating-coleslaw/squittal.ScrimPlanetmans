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
        }

        private string GetInfo()
        {
            var teamOrdinal = FacilityControl.ControllingTeamOrdinal;

            var actionDisplay = GetEnumValueName(FacilityControl.ActionType);
            var controlTypeDisplay = Enum.GetName(typeof(FacilityControlType), FacilityControl.ControlType).ToUpper();

            var facilityName = FacilityControl.FacilityName;
            var facilityId = FacilityControl.FacilityId;

            var pointsDisplay = GetPointsDisplay(FacilityControl.Points);

            return $"Team {teamOrdinal} {actionDisplay} {controlTypeDisplay}: {pointsDisplay} {facilityName} [{facilityId}]";
        }
    }
}
