using squittal.ScrimPlanetmans.Models.Planetside;
using squittal.ScrimPlanetmans.Services.Planetside;
using System.Text.RegularExpressions;

namespace squittal.ScrimPlanetmans.ScrimMatch.Models
{
    public class Player : IEquitable<Player>
    {
        public string Id { get; }

        public int TeamOrdinal { get; set; }

        public ScrimEventAggregate EventAggregate
        {
            get
            {
                return EventAggregateTracker.TotalStats;
            }
        }

        public ScrimEventAggregateRoundTracker EventAggregateTracker { get; set; } = new ScrimEventAggregateRoundTracker();

        public string NameFull { get; set; }
        public string NameTrimmed { get; set; }
        public string NameAlias { get; set; }

        //public string NameDisplay { get; set; }
        public string NameDisplay
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(NameAlias))
                {
                    return NameAlias;
                }
                else if (!string.IsNullOrWhiteSpace(NameTrimmed))
                {
                    return NameTrimmed;
                }
                else
                {
                    return NameFull;
                }
            }
        }

        public int FactionId { get; set; }
        public int WorldId { get; set; }

        public int PrestigeLevel { get; set; }

        public string OutfitId { get; set; } = string.Empty;
        public string OutfitAlias { get; set; } = string.Empty;
        public string OutfitAliasLower { get; set; } = string.Empty;

        public bool IsOutfitless { get; set; } = false;

        public int? ConstructedTeamId { get; set; }

        public bool IsFromConstructedTeam => ConstructedTeamId != null;

        // Dynamic Attributes
        public int? LoadoutId { get; set; }
        public PlayerStatus Status { get; set; } = PlayerStatus.Unknown;

        public bool IsOnline { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public bool IsParticipating { get; set; } = false;
        public bool IsBenched { get; set; } = false;

        private static readonly Regex _nameRegex = new Regex("^[A-Za-z0-9]{1,32}$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        // Format for Planetside Infantry League: Season 2 => Namex##
        private static readonly Regex _pil2NameRegex = new Regex("^[A-z0-9]{2,}(x[0-9]{2})$", RegexOptions.Compiled);

        // Format for Legacy Jaeger Characters => TAGxName(VS|NC|TR)
        private static readonly Regex _legacyJaegerNameRegex = new Regex("^([A-z0-9]{0,4}x).{2,}(?<!(x[0-9]{2}))$", RegexOptions.Compiled);

        private static readonly Regex _factionSufficRegex = new Regex("^[A-z0-9]+(VS|NC|TR)$", RegexOptions.Compiled);

        public Player(Character character)
        {
            Id = character.Id;
            NameFull = character.Name;
            //NameDisplay = NameTrimmed;
            IsOnline = character.IsOnline;
            PrestigeLevel = character.PrestigeLevel;
            FactionId = character.FactionId;
            WorldId = character.WorldId;
            OutfitId = character.OutfitId;
            OutfitAlias = character.OutfitAlias;
            OutfitAliasLower = character.OutfitAliasLower;

            // Last because it requires WorldId being set
            NameTrimmed = GetTrimmedPlayerName(NameFull, WorldId);
        }

        public static string GetTrimmedPlayerName(string name, int worldId)
        {
            var isPil2NameFormat = false;
            var isLegacyJaegerNameFormat = false;

            if (WorldService.IsJaegerWorldId(worldId))
            {
                if (_pil2NameRegex.Match(name).Success)
                {
                    isPil2NameFormat = true;
                }
                else if (_legacyJaegerNameRegex.Match(name).Success)
                {
                    isLegacyJaegerNameFormat = true;
                }
            }

            var trimmed = name;
            var initLength = name.Length;

            if (isPil2NameFormat)
            {
                trimmed = name.Substring(0, initLength - 3);
            }
            else if (isLegacyJaegerNameFormat)
            {
                // Remove outfit tag from beginning of name
                var idx = name.IndexOf("x");
                if (idx >= 0 && idx < 5 && (idx != initLength - 1))
                {
                    trimmed = name.Substring(idx + 1, initLength - idx - 1);
                }
            }

            if (!isPil2NameFormat && _factionSufficRegex.Match(trimmed).Success)
            {
                // Remove faction abbreviation from end of name
                var end = trimmed.Length - 2;
                trimmed = trimmed.Substring(0, end);
            }

            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.Length <= 1)
            {
                trimmed = name;
            }

            return trimmed;
        }

        public void UpdateNameTrimmed()
        {
            NameTrimmed = GetTrimmedPlayerName(NameFull, WorldId);
        }

        public bool TrySetNameAlias(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return false;
            }
            
            Match match = _nameRegex.Match(alias);
            if (!match.Success)
            {
                return false;
            }

            NameAlias = alias;

            return true;
        }

        public void ClearAllDisplayNameSources()
        {
            NameAlias = string.Empty;
            NameTrimmed = string.Empty;
        }

        public void AddStatsUpdate(ScrimEventAggregate update)
        {
            EventAggregateTracker.AddToCurrent(update);
        }

        public void SubtractStatsUpdate(ScrimEventAggregate update)
        {
            EventAggregateTracker.SubtractFromCurrent(update);
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


    public enum PlayerStatus
    {
        Unknown,
        Alive,
        Respawning,
        Revived,
        ContestingObjective,
        Benched,
        Offline
    }
}
