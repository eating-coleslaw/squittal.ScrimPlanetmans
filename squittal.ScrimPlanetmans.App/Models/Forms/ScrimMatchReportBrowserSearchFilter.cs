using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class ScrimMatchReportBrowserSearchFilter
    {
        public DateTime? SearchStartDate { get; set; } = _defaultSearchStartDate; // new DateTime(2012,11, 20); // PlanetSide 2 release date
        public DateTime? SearchEndDate { get; set; } = _defaultSearchEndDate; //DateTime.UtcNow.AddDays(1);
        //public int WorldId { get; set; } = -1;
        //public int FacilityId { get; set; } = -1;
        public int WorldId { get => GetWorldIdFromString(); }
        public string WorldIdString { get; set; } = _defaultSearchWorldIdString; //"19";
        public int FacilityId { get => GetFacilityIdFromString(); }
        public string FacilityIdString { get; set; } = _defaultSearchFacilityIdString; //"0";
        public int MinimumRoundCount { get; set; } = _defaultSearchMinimumRoundCount; //2;

        public string InputSearchTerms { get; set; } = _defaultSearchInputTerms; // string.Empty;

        public List<string> SearchTermsList { get; private set; } = new List<string>();
        public List<string> AliasSearchTermsList { get; private set; } = new List<string>();

        public string PrimaryTeamAlias { get; set; } = string.Empty;
        public string SecondaryTeamAlias { get; set; } = string.Empty;
        //public OutfitAlias PrimaryTeamAlias { get; set; } = new OutfitAlias();
        //public OutfitAlias SecondaryTeamAlias { get; set; } = new OutfitAlias();

        private readonly AutoResetEvent _searchTermsAutoEvent = new AutoResetEvent(true);
        private readonly AutoResetEvent _worldAutoEvent = new AutoResetEvent(true);
        private readonly AutoResetEvent _facilityAutoEvent = new AutoResetEvent(true);

        private static readonly DateTime _defaultSearchStartDate = new DateTime(2012, 11, 20); // PlanetSide 2 release date
        private static readonly DateTime _defaultSearchEndDate = DateTime.UtcNow.AddDays(1);
        private static readonly string _defaultSearchWorldIdString = "19"; // Jaeger
        private static readonly string _defaultSearchFacilityIdString = "0"; // Any Facility
        private static readonly int _defaultSearchMinimumRoundCount = 2;
        private static readonly string _defaultSearchInputTerms = string.Empty;
        

        public bool IsDefaultFilter => GetIsDefaultFilter();

        private static Regex TeamAliasRegex { get; } = new Regex("^[A-Za-z0-9]{1,4}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private bool GetIsDefaultFilter()
        {
            return MinimumRoundCount == _defaultSearchMinimumRoundCount
                   && WorldIdString == _defaultSearchWorldIdString
                   && FacilityIdString == _defaultSearchFacilityIdString
                   && SearchStartDate == _defaultSearchStartDate && SearchEndDate == _defaultSearchEndDate
                   && InputSearchTerms == _defaultSearchInputTerms
                   && !SearchTermsList.Any() && !AliasSearchTermsList.Any();
        }

        public void ParseSearchTermsString()
        {
            //_searchTermsAutoEvent.WaitOne();

            var searchTerms = InputSearchTerms;

            SearchTermsList = new List<string>();
            AliasSearchTermsList = new List<string>();
            
            if (string.IsNullOrWhiteSpace(searchTerms))
            {
                return;
            }

            var splitTerms = searchTerms.Split(' ');

            foreach (var term in splitTerms)
            {
                var termLower = term.ToLower();

                if (TeamAliasRegex.Match(termLower).Success && !AliasSearchTermsList.Contains(termLower) && termLower != "vs" && termLower != "ps2")
                {
                    AliasSearchTermsList.Add(termLower);
                }
                if (!SearchTermsList.Contains(termLower) && termLower.Length > 1)
                {
                    SearchTermsList.Add(termLower);
                }
            }

            //_searchTermsAutoEvent.Set();
        }

        public bool SetWorldId(int worldId)
        {
            if (worldId <= -1)
            {
                return false;
            }

            SetWorldId(worldId.ToString());

            return true;
        }

        public void SetWorldId(string worldIdString)
        {
            _worldAutoEvent.WaitOne();

            WorldIdString = worldIdString;
            _worldAutoEvent.Set();
        }

        public bool SetFacilityId(int facilityId)
        {
            if (facilityId <= 0)
            {
                return false;
            }

            SetFacilityId(facilityId.ToString());

            return true;
        }

        public void SetFacilityId(string facilityIdString)
        {
            _facilityAutoEvent.WaitOne();

            FacilityIdString = facilityIdString;
            _facilityAutoEvent.Set();
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
