using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Linq;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class Team
    {
        public string Alias { get; set; } //first seed team, or a custom value
        public string NameInternal { get; set; } //team1 or team2
        public int TeamOrdinal { get; private set; } //1 or 2
        public int? FactionId { get; set; }

        public ScrimEventAggregate EventAggregate
        {
            get
            {
                return EventAggregateTracker.TotalStats;
            }
        }

        public ScrimEventAggregateRoundTracker EventAggregateTracker { get; set; } = new ScrimEventAggregateRoundTracker();

        public List<Player> Players { get; } = new List<Player>();
        public List<Player> ActivePlayers { get; } = new List<Player>();
        public List<Player> ParticipatingPlayers { get; set; } = new List<Player>();

        public List<Outfit> Outfits { get => _outfits; }
        public List<Player> NonOutfitCharacters { get; } = new List<Player>();

        public List<ConstructedTeam> ConstructedTeams { get; set; } = new List<ConstructedTeam>();

        public List<string> PlayerIds { get => _playerIds; }
        public List<string> PlayersIdsOnline { get => _playerIdsOnline; }

        public List<Character> Characters { get => _playerIdMap.Values.ToList(); }

        public Dictionary<string, Outfit> CharacterIdOutfitMap { get => _playerOutfitMap; }

        private List<string> _seedOutfitAliases = new List<string>();
        private List<string> _seedOutfitIds = new List<string>();

        public bool HasCustomAlias { get; private set; } = false;

        private List<string> _playerIds = new List<string>();
        private List<string> _playerIdsOnline = new List<string>();
        private Dictionary<string, Character> _playerIdMap = new Dictionary<string, Character>();

        private List<Outfit> _outfits = new List<Outfit>();
        private Dictionary<string, Outfit> _playerOutfitMap = new Dictionary<string, Outfit>(); // <characterId, alias>

        public Team(string alias, string nameInternal, int teamOrdinal)
        {
            TrySetAlias(alias, false);
            NameInternal = nameInternal;
            TeamOrdinal = teamOrdinal;
        }

        public bool TrySetAlias(string alias, bool isCustomAlias = false)
        {
            // Don't overwrite a custom display alias unless the new one is also custom
            if (!HasCustomAlias || isCustomAlias)
            {
                Alias = alias;
                HasCustomAlias = isCustomAlias;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ContainsPlayer(string characterId)
        {
            return _playerIds.Contains(characterId);
        }

        public bool ContainsOutfit(string alias)
        {
            return _seedOutfitAliases.Contains(alias);
        }

        public bool ContainsConstructedTeam(int constructedTeamId)
        {
            return ConstructedTeams.Any(ct => ct.Id == constructedTeamId);
        }

        public bool TryAddPlayer(Player player)
        {
            if (ContainsPlayer(player.Id))
            {
                return false;
            }

            Players.Add(player);

            _playerIds.Add(player.Id);

            return true;
        }

        public bool TryRemovePlayer(string characterId)
        {
            if (!ContainsPlayer(characterId))
            {
                return false;
            }

            var player = Players.FirstOrDefault(p => p.Id == characterId);

            Players.RemoveAll(p => p.Id == characterId);
            _playerIds.RemoveAll(id => id == characterId);

            ParticipatingPlayers.RemoveAll(p => p.Id == characterId);

            EventAggregateTracker.SubtractFromHistory(player.EventAggregateTracker);

            return true;
        }


        public bool TryAddOutfit(Outfit outfit)
        {
            if (ContainsOutfit(outfit.AliasLower))
            {
                return false;
            }

            Outfits.Add(outfit);
            _seedOutfitAliases.Add(outfit.AliasLower);
            _seedOutfitIds.Add(outfit.Id);
            
            return true;
        }

        public bool TryRemoveOutfit(string aliasLower)
        {
            var outfit = Outfits.FirstOrDefault(o => o.AliasLower == aliasLower);
            
            if (outfit == null)
            {
                return false;
            }

            Outfits.RemoveAll(o => o.AliasLower == aliasLower);
            _seedOutfitAliases.RemoveAll(alias => alias == aliasLower);
            _seedOutfitIds.RemoveAll(id => id == outfit.Id);

            return true;
        }

        
        public bool TryAddConstructedTeam(ConstructedTeam constructedTeam)
        {
            if (ContainsConstructedTeam(constructedTeam.Id))
            {
                return false;
            }

            ConstructedTeams.Add(constructedTeam);
            return true;
        }

        public bool TryRemoveConstructedTeam(int constructedTeamId)
        {
            if (!ContainsConstructedTeam(constructedTeamId))
            {
                return false;
            }

            ConstructedTeams.RemoveAll(ct => ct.Id == constructedTeamId);
            return true;
        }

        public IEnumerable<Player> GetOutfitPlayers(string aliasLower)
        {
            lock(Players)
            {
                return Players.Where(p => p.OutfitAliasLower == aliasLower && !p.IsOutfitless && !p.IsFromConstructedTeam).ToList();
            }
        }

        public void AddStatsUpdate(ScrimEventAggregate update)
        {
            EventAggregateTracker.AddToCurrent(update);
        }

        public void SubtractStatsUpdate(ScrimEventAggregate update)
        {
            EventAggregateTracker.SubtractFromCurrent(update);
        }

        public void ClearEventAggregateHistory()
        {
            EventAggregateTracker = new ScrimEventAggregateRoundTracker();
        }

        public void RollBackPlayerRoundStats(string characterId, int currentRound)
        {
            EventAggregateTracker.RollBackRound(currentRound);
        }
    }
}
