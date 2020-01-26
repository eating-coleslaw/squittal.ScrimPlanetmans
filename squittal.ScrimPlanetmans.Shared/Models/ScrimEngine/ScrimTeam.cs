using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Collections.Generic;

namespace squittal.ScrimPlanetmans.Shared.Models.ScrimEngine
{
    public class ScrimTeam
    {
        public List<string> SeedOutfitAliases { get; set; }
        public List<Outfit> Outfits { get; private set; }
        private List<string> VerifiedOutfitAliases { get; set; } = new List<string>();


        public List<string> SeedCharacterIds { get; set; }
        public List<string> SeedCharacterNames { get; set; }
        public List<Character> Characters { get; private set; }
        public Dictionary<string, Character> CharacterInfoMap { get; private set; }
        public Dictionary<string, ScrimEventAggregate> CharacterStatsMap { get; private set; }
    }
}
