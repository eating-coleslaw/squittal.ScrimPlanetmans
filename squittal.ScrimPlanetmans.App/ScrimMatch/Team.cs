using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Shared.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System.Collections.Generic;
using System.Linq;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class Team
    {
        public string Alias { get; set; } //first seed team, or a custom value
        public string NameInternal { get; set; } //team1 or team2
        public int TeamOrdinal { get; private set; } //1 or 2
        public int? FactionId { get; set; }

        public ScrimEventAggregate EventAggregate { get; set; } = new ScrimEventAggregate();

        // Each aggregate is only the points scored during the round number of the enytry's key
        public Dictionary<int, ScrimEventAggregate> EventAggregateRoundHistory = new Dictionary<int, ScrimEventAggregate>();

        public List<Player> Players { get; } = new List<Player>();
        public List<Player> ActivePlayers { get; } = new List<Player>();
        public List<Player> ParticipatingPlayers { get; set; } = new List<Player>();

        public List<Outfit> Outfits { get => _outfits; }

        public List<string> PlayerIds { get => _playerIds; }
        public List<string> PlayersIdsOnline { get => _playerIdsOnline; }

        public List<Character> Characters { get => _playerIdMap.Values.ToList(); }

        public Dictionary<string, Outfit> CharacterIdOutfitMap { get => _playerOutfitMap; }

        private List<string> _seedOutfitAliases = new List<string>();
        private List<string> _seedOutfitIds = new List<string>();

        private List<string> _playerIds = new List<string>();
        private List<string> _playerIdsOnline = new List<string>();
        private Dictionary<string, Character> _playerIdMap = new Dictionary<string, Character>();

        private List<string> _loadingAliases = new List<string>();
        private List<string> _loadingPlayerIds = new List<string>();

        private List<Outfit> _outfits = new List<Outfit>();
        private Dictionary<string, Outfit> _playerOutfitMap = new Dictionary<string, Outfit>(); // <characterId, alias>

        public Team(string alias, string nameInternal, int teamOrdinal)
        {
            Alias = alias;
            NameInternal = nameInternal;
            TeamOrdinal = teamOrdinal;

            //EventAggregate = new ScrimEventAggregate();
        }

        public bool ContainsPlayer(string characterId)
        {
            return _playerIds.Contains(characterId);
        }

        public bool ContainsOutfit(string alias)
        {
            return _seedOutfitAliases.Contains(alias);
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

            EventAggregate.Subtract(player.EventAggregate);

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

        public void SaveRoundToEventAggregateHistory(int round)
        {
            if (round < 1)
            {
                return;
            }

            var maxRound = GetHighestEventAggregateHistoryRound();

            // Only allow updating the current round, or saving a new round
            if (round != maxRound && round != (maxRound + 1))
            {
                return;
            }

            var roundStats = new ScrimEventAggregate();

            roundStats.Add(EventAggregate);

            for (var r = 1; r == (round - 1); r++)
            {
                if (EventAggregateRoundHistory.TryGetValue(r, out ScrimEventAggregate stats))
                {
                    roundStats.Subtract(stats);
                }
            }

            if (EventAggregateRoundHistory.ContainsKey(round))
            {
                EventAggregateRoundHistory[round] = roundStats;
            }
            else
            {
                EventAggregateRoundHistory.Add(round, roundStats);
            }
        }

        private int GetHighestEventAggregateHistoryRound()
        {
            var rounds = EventAggregateRoundHistory.Keys.ToArray();

            if (!rounds.Any())
            {
                return 0;
            }

            return rounds.Max();
        }

        private void AddOutfitPlayersToTeam(Outfit outfit, IEnumerable<Character> characters)
        {
            var alias = outfit.Alias;
            var outfitId = outfit.Id;
            
            if (!_seedOutfitAliases.Contains(alias))
            {
                _seedOutfitAliases.Add(alias);
                _seedOutfitIds.Add(outfitId);

                _outfits.Add(outfit);

                var characterIds = characters.Select(c => c.Id).ToList();
                var newCharacters = characters.Where(c => !PlayerIds.Contains(c.Id)).ToList();

                _playerIds.AddRange(newCharacters.Select(c => c.Id));
                _playerIdsOnline.AddRange(characters.Where(c => c.IsOnline == true).Select(c => c.Id));

                foreach (var character in characters)
                {
                    var id = character.Id;
                    
                    if (!_playerIdMap.ContainsKey(id))
                    {
                        _playerIdMap.Add(id, character);
                    }
                    else
                    {
                        _playerIdMap[id] = character;
                    }

                    if (!_playerOutfitMap.ContainsKey(id))
                    {
                        _playerOutfitMap.Add(id, outfit);
                    }
                    else
                    {
                        _playerOutfitMap[id] = outfit;
                    }

                    if (character.IsOnline)
                    {
                        if (!_playerIdsOnline.Contains(id))
                        {
                            _playerIdsOnline.Add(id);
                        }
                    }
                    else
                    {
                        _playerIdsOnline.Remove(id);
                    }
                }
            }
        }
    }
}
