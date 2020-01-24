using System;

namespace squittal.ScrimPlanetmans.CensusServices.Models
{
    public class CensusCharacterModel
    {
        public string CharacterId { get; set; }
        public CharacterName Name { get; set; }
        public int FactionId { get; set; }
        public int TitleId { get; set; }
        public CharacterTimes Times { get; set; }
        public CharacterBattleRank BattleRank { get; set; }
        public CharacterCerts Certs { get; set; }
        public int WorldId { get; set; }
        public bool OnlineStatus { get; set; }
        public int PrestigeLevel { get; set; }

        public class CharacterName
        {
            public string First { get; set; }
            public string FirstLower { get; set; }
        }

        public class CharacterTimes
        {
            public DateTime CreationDate { get; set; }
            public DateTime LastSaveDate { get; set; }
            public DateTime LastLoginDate { get; set; }
            public int MinutesPlayed { get; set; }
        }

        public class CharacterBattleRank
        {
            public int PercentToNext { get; set; }
            public int Value { get; set; }
        }

        public class CharacterCerts
        {
            public int EarnedPoints { get; set; }
            public int GiftedPoints { get; set; }
            public int SpentPoints { get; set; }
            public int AvailablePoints { get; set; }
            public float PercentToNext { get; set; }
        }
    }
}
