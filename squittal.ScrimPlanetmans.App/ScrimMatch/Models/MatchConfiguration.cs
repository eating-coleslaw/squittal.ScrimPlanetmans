using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace squittal.ScrimPlanetmans.Models.ScrimEngine
{
    public class MatchConfiguration
    {
        public string Title { get; set; } = "PS2 Scrims";
        public int RoundSecondsTotal { get; set; } = 900;

        // Target Base Configuration
        public bool IsManualWorldId { get; private set; } = false;
        public bool IsWorldIdSet { get; private set; } = false;
        public int WorldId { get => GetWorldIdFromString(); }
        public string WorldIdString { get; set; } = "19";
        public int FacilityId { get => GetFacilityIdFromString(); }
        public string FacilityIdString { get; set; } = "-1";

        public bool EndRoundOnFacilityCapture { get; set; } = false; // TODO: move this setting to the Ruleset model

        private readonly AutoResetEvent _autoEvent = new AutoResetEvent(true);

        public bool SaveLogFiles { get; set; } = true;

        public void ResetWorldId()
        {
            WorldIdString = "19";
            IsManualWorldId = false;
            IsWorldIdSet = false;
        }
        
        public bool TrySetWorldId(int worldId, bool isManualValue = false, bool isRollBack = false)
        {
            if (worldId <= 0)
            {
                return false;
            }
            return TrySetWorldId(worldId.ToString(), isManualValue, isRollBack);
        }

        public bool TrySetWorldId(string worldIdString, bool isManualValue = false, bool isRollBack = false)
        {
            _autoEvent.WaitOne();

            if (isManualValue)
            {
                WorldIdString = worldIdString;
                IsManualWorldId = true;
                IsWorldIdSet = true;

                _autoEvent.Set();

                return true;
            }
            else if (!IsManualWorldId && (!IsWorldIdSet || isRollBack))
            {
                WorldIdString = worldIdString;

                IsWorldIdSet = true;

                _autoEvent.Set();

                return true;
            }
            else
            {
                _autoEvent.Set();

                return false;
            }
        }

        private int GetFacilityIdFromString()
        {
            if (int.TryParse(FacilityIdString, out int intId))
            {
                return intId;
            }
            else
            {
                return -1;
            }
        }

        private int GetWorldIdFromString()
        {
            if (int.TryParse(WorldIdString, out int intId))
            {
                return intId;
            }
            else
            {
                return 19; // Default to Jaeger
            }
        }
    }
}
