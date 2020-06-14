namespace squittal.ScrimPlanetmans.Models.Forms
{
    public class ConstructedTeamFactionSelection
    {
        public string ConstructedTeamStringId { get; set; } = string.Empty;

        public int ConstructedTeamId
        {
            get
            {
                if (int.TryParse(ConstructedTeamStringId, out int intId))
                {
                    return intId;
                }
                else
                {
                    return -1;
                }
            }
        }

        public string FactionStringId { get; set; } = _defaultFactionStringId;

        public int FactionId
        {
            get
            {
                if (int.TryParse(FactionStringId, out int intId))
                {
                    return intId;
                }
                else
                {
                    return _defaultFactionId;
                }
            }
        }

        private static readonly string _defaultFactionStringId = "1";
        private static readonly int _defaultFactionId = 1;
    }
}
