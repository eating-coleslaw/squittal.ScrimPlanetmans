using squittal.ScrimPlanetmans.Shared.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class Player : IEquitable<Player>
    {
        public string Id { get; }

        //public Team Team { get; set; }
        public int TeamOrdinal { get; set; }

        public ScrimEventAggregate EventAggregate;

        public string NameFull { get; set; }
        public string NameDisplay { get; set; }
        public string NameTrimmed { get; set; }
        public string NameAlias { get; set; }

        public int FactionId { get; set; }

        public string OutfitId { get; set; } = string.Empty;
        public string OutfitAlias { get; set; } = string.Empty;
        public string OutfitAliasLower { get; set; } = string.Empty;

        // Dynamic Attributes
        public int LoadoutId { get; set; } = 0;

        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
        public bool IsParticipating { get; set; }

        public Player(Character character)
        {
            Id = character.Id;
            NameFull = character.Name;
            NameTrimmed = GetTrimmedPlayerName(NameFull);
            NameDisplay = NameTrimmed;
            IsOnline = character.IsOnline;
            FactionId = character.FactionId;
            OutfitId = character.OutfitId;
            OutfitAlias = character.OutfitAlias;
            OutfitAliasLower = character.OutfitAliasLower;

            EventAggregate = new ScrimEventAggregate();
        }

        private static string GetTrimmedPlayerName(string name)
        {
            var trimmed = name;

            try
            {
                // Remove outfit tag from beginning of name
                var idx = name.IndexOf("x");
                if (idx > 0 && idx < 5 && (idx != name.Length - 1))
                {
                    trimmed = name.Substring(idx + 1, name.Length - idx - 1);
                }

                // Remove faction abbreviation from end of name
                var end = trimmed.Length - 2;
                if (trimmed.IndexOf("VS") == end || trimmed.IndexOf("NC") == end || trimmed.IndexOf("TR") == end)
                {
                    trimmed = trimmed.Substring(0, end);
                }
            }
            catch
            {
                return name;
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

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Player); 
        }

        public bool Equals(Player p)
        {
            if (ReferenceEquals(p, null))
            {
                return false;
            }

            if (ReferenceEquals(this, p))
            {
                return true;
            }

            if (this.GetType() != p.GetType())
            {
                return false;
            }

            return p.Id == Id;
        }

        public static bool operator ==(Player lhs, Player rhs)
        {
            if (ReferenceEquals(lhs, null))
            {
                if (ReferenceEquals(rhs, null))
                {
                    return true;
                }

                return false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Player lhs, Player rhs)
        {
            return !(lhs == rhs);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
