using squittal.ScrimPlanetmans.Shared.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class Player
    {
        public string Id { get; }
        public Team Team { get; set; }

        public ScrimEventAggregate EventAggregate = new ScrimEventAggregate();

        public string NameFull { get; set; }
        public string NameDisplay { get; set; }
        public string NameTrimmed { get; set; }
        public string NameAlias { get; set; }
        public string OutfitId { get; set; }

        // Dynamic Attributes
        public int LoadoutId { get; set; } = 0;
        public bool IsActive { get; set; }

        public Player(Character character)
        {
            Id = character.Id;
            NameFull = character.Name;
            NameTrimmed = GetTrimmedPlayerName(NameFull);
            NameDisplay = NameTrimmed;
            //OutfitId = character.OutfitId;
        }

        private static string GetTrimmedPlayerName(string name)
        {
            var trimmed = name;

            // Remove outfit tag from beginning of name
            var idx = name.IndexOf("x");
            if (idx > 0 && idx < 5 && (idx != name.Length - 1))
            {
                trimmed = name.Substring(idx + 1, name.Length);
            }

            // Remove faction abbreviation from end of name
            var end = trimmed.Length - 2;
            if (trimmed.IndexOf("VS") == end || trimmed.IndexOf("NC") == end || trimmed.IndexOf("TR") == end)
            {
                trimmed = trimmed.Substring(0, end);
            }
            
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                trimmed = name;
            }

            return trimmed;
        }

        public void SetNameAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentNullException();
            }

            NameAlias = alias;
            NameDisplay = NameAlias;
        }
    }
}
