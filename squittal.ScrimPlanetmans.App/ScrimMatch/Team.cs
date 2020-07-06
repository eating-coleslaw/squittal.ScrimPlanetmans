using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Models.Planetside;
using System.Collections.Generic;
using System.Linq;
using squittal.ScrimPlanetmans.Data.Models;

using System.Collections.Concurrent;

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

        public List<Player> Players { get; private set; } = new List<Player>();

        public List<Player> ParticipatingPlayers { get; set; } = new List<Player>();

        private ConcurrentDictionary<string, Player> ParticipatingPlayersMap { get; set; } = new ConcurrentDictionary<string, Player>();

        //public List<Outfit> Outfits { get => _outfits; }
        public List<Outfit> Outfits { get; private set; } = new List<Outfit>();

        public List<ConstructedTeamMatchInfo> ConstructedTeamsMatchInfo { get; set; } = new List<ConstructedTeamMatchInfo>();
        private ConcurrentDictionary<string, ConstructedTeamMatchInfo> ConstructedTeamsMap { get; set; } = new ConcurrentDictionary<string, ConstructedTeamMatchInfo>();

        private ConcurrentDictionary<string, Player> PlayersMap { get; set; } = new ConcurrentDictionary<string, Player>();
        //public List<string> PlayerIds { get => _playerIds; }

        private ConcurrentDictionary<string, Outfit> OutfitsMap { get; set; } = new ConcurrentDictionary<string, Outfit>();
        //private List<string> _seedOutfitAliases = new List<string>();
        
        //private List<string> _seedOutfitIds = new List<string>();

        public bool HasCustomAlias { get; private set; } = false;

        //private List<string> _playerIds = new List<string>();

        //private List<Outfit> _outfits = new List<Outfit>();

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
            //return _playerIds.Contains(characterId);
            return PlayersMap.ContainsKey(characterId);
        }

        public bool ContainsOutfit(string alias)
        {
            //return _seedOutfitAliases.Contains(alias);
            return OutfitsMap.ContainsKey(alias.ToLower());
        }

        public bool ContainsConstructedTeamFaction(int constructedTeamId, int factionId)
        {
            //return ConstructedTeamsMatchInfo.Any(ctmi => ctmi.ConstructedTeam.Id == constructedTeamId && ctmi.ActiveFactionId == factionId);
            return ConstructedTeamsMap.ContainsKey(GetConstructedTeamFactionKey(constructedTeamId, factionId));
        }

        public IEnumerable<string> GetAllPlayerIds()
        {
            return PlayersMap.Keys.ToList();
        }

        public bool TryGetPlayerFromId(string characterId, out Player player)
        {
            return PlayersMap.TryGetValue(characterId, out player);
        }

        public bool TryAddPlayer(Player player)
        {
            //if (ContainsPlayer(player.Id))
            //{
            //    return false;
            //}

            if(!PlayersMap.TryAdd(player.Id, player))
            {
                return false;
            }

            Players.Add(player);

            //_playerIds.Add(player.Id);

            //PlayersMap.TryAdd(player.Id, player);

            return true;
        }

        public bool TryRemovePlayer(string characterId)
        {
            //if (!ContainsPlayer(characterId))
            //{
            //    return false;
            //}

            //var player = Players.FirstOrDefault(p => p.Id == characterId);
            if (!PlayersMap.TryRemove(characterId, out var player))
            {
                return false;
            }

            //Players.RemoveAll(p => p.Id == characterId);
            Players.Remove(player);

            //_playerIds.RemoveAll(id => id == characterId);
            //PlayersMap.TryRemove(characterId, out var playerOut);

            //ParticipatingPlayers.RemoveAll(p => p.Id == characterId);
            ParticipatingPlayers.Remove(player);

            ParticipatingPlayersMap.TryRemove(player.Id, out Player removedPlayer);

            EventAggregateTracker.SubtractFromHistory(player.EventAggregateTracker);

            return true;
        }

        public bool TryAddOutfit(Outfit outfit)
        {
            //if (ContainsOutfit(outfit.AliasLower))
            //{
            //    return false;
            //}

            if (!OutfitsMap.TryAdd(outfit.AliasLower, outfit))
            {
                return false;
            }

            Outfits.Add(outfit);

            //_seedOutfitAliases.Add(outfit.AliasLower);
            //OutfitsMap.TryAdd(outfit.AliasLower, outfit);
            
            //_seedOutfitIds.Add(outfit.Id);
            
            return true;
        }

        public bool TryRemoveOutfit(string aliasLower)
        {
            if (!OutfitsMap.TryRemove(aliasLower, out var outfitOut))
            {
                return false;
            }
            
            //var outfit = Outfits.FirstOrDefault(o => o.AliasLower == aliasLower);
            
            //if (outfit == null)
            //{
                //return false;
            //}

            Outfits.RemoveAll(o => o.AliasLower == aliasLower);

            //_seedOutfitAliases.RemoveAll(alias => alias == aliasLower);
            //OutfitsMap.TryRemove(aliasLower, out var outfitOut);
            
            //_seedOutfitIds.RemoveAll(id => id == outfit.Id);

            return true;
        }

        public bool TryAddConstructedTeamFaction(ConstructedTeamMatchInfo matchInfo)
        {
            var constructedTeam = matchInfo.ConstructedTeam;
            var factionId = matchInfo.ActiveFactionId;

            if (!ConstructedTeamsMap.TryAdd(GetConstructedTeamFactionKey(constructedTeam.Id, factionId), matchInfo))
            {
                return false;
            }

            //if (ContainsConstructedTeamFaction(constructedTeam.Id, factionId))
            //{
            //    return false;
            //}

            ConstructedTeamsMatchInfo.Add(matchInfo);

            //ConstructedTeamsMap.TryAdd(GetConstructedTeamFactionKey(constructedTeam.Id, factionId), matchInfo);

            return true;
        }

        public bool TryRemoveConstructedTeamFaction(int constructedTeamId, int factionId)
        {
            //if (!ContainsConstructedTeamFaction(constructedTeamId, factionId))
            //{
            //    return false;
            //}

            if (!ConstructedTeamsMap.TryRemove(GetConstructedTeamFactionKey(constructedTeamId, factionId), out var teamOut))
            {
                return false;
            }

            ConstructedTeamsMatchInfo.RemoveAll(ctmi => ctmi.ConstructedTeam.Id == constructedTeamId && ctmi.ActiveFactionId == factionId);

            //ConstructedTeamsMap.TryRemove(GetConstructedTeamFactionKey(constructedTeamId, factionId), out var teamOut);

            return true;
        }

        public bool UpdateParticipatingPlayer(Player player)
        {
            var playerId = player.Id;

            if (player.IsParticipating)
            {
                return ParticipatingPlayersMap.TryAdd(playerId, player);
            }
            else
            {
                return ParticipatingPlayersMap.TryRemove(playerId, out Player removedPlayer);
            }
        }

        public IEnumerable<Player> GetParticipatingPlayers()
        {
            return ParticipatingPlayersMap.Values.ToList();
            
            /*
            var participatingPlayers = new List<Player>();

            foreach (var playerPair in ParticipatingPlayersMap)
            {
                    participatingPlayers.Add(playerPair.Value);
            }

            //foreach (var playerPair in ParticipatingPlayersMap)
            //{
            //    if (playerPair.Value)
            //    {
            //        if (PlayersMap.TryGetValue(playerPair.Key, out var player))
            //        {
            //            participatingPlayers.Add(player);
            //        }
            //    }
            //}

            return participatingPlayers;
            */
        }

        public IEnumerable<Player> GetOutfitPlayers(string aliasLower)
        {
            lock(Players)
            {
                return Players.Where(p => p.OutfitAliasLower == aliasLower && !p.IsOutfitless && !p.IsFromConstructedTeam).ToList();
            }
        }

        public IEnumerable<Player> GetNonOutfitPlayers()
        {
            lock (Players)
            {
                return Players.Where(p => p.IsOutfitless && !p.IsFromConstructedTeam).ToList();
            }
        }

        public IEnumerable<Player> GetConstructedTeamFactionPlayers(int constructedTeamId, int factionId)
        {
            lock (Players)
            {
                return Players
                        .Where(p => p.IsFromConstructedTeam && p.ConstructedTeamId == constructedTeamId && p.FactionId == factionId)
                        .ToList();
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

        private string GetConstructedTeamFactionKey(int constructedTeamId, int factionId)
        {
            return $"{constructedTeamId}^{factionId}";
        }
    }
}
