using Newtonsoft.Json.Linq;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Models.Planetside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using squittal.ScrimPlanetmans.Models.Forms;

namespace squittal.ScrimPlanetmans.ScrimMatch
{
    public class ScrimPlayersManager : IScrimPlayersManager
    {
        private readonly IOutfitService _outfits;
        private readonly ICharacterService _characters;
        //private readonly IWebsocketEventHandler _eventHandler;

        private Dictionary<string, List<Character>> _membersMap = new Dictionary<string, List<Character>>();
        private Dictionary<string, List<Character>> _membersOnlineMap = new Dictionary<string, List<Character>>();
        private Dictionary<string, string> _outfitMap = new Dictionary<string, string>(); //<characterId, alias>

        private List<Character> _outfitMembers = new List<Character>();
        private List<Character> _outfitOnlineMembers = new List<Character>();

        private List<string> _isAliasLoading = new List<string>();
        private List<string> _isMembersLoading = new List<string>();

        private List<Outfit> _teamOutfits1 = new List<Outfit>();
        private List<string> _validOutfitAliasHistory = new List<string>();
        private List<string> _subscribedOutfitsHistory = new List<string>();

        public ScrimPlayersManager(IOutfitService outfits, ICharacterService characters) //, IWebsocketMonitor wsMonitor)
        {
            _outfits = outfits;
            _characters = characters;
            //_wsMonitor = wsMonitor;
        }

        public async Task<Player> GetPlayerFromCharacterId(string characterId)
        {
            var character = await _characters.GetCharacterAsync(characterId);

            if (character == null)
            {
                return null;
            }

            return new Player(character);
        }

        public async Task<IEnumerable<Player>> GetPlayersFromOutfitAlias(string alias)
        {
            var censusMembers = await _outfits.GetOutfitMembersByAlias(alias);

            if (censusMembers == null || !censusMembers.Any())
            {
                return null;
            }

            return censusMembers.Select(m => new Player(m)).ToList();
        }

        private void HandlePlayerLoginPayload(JToken payload)
        {
            var characterId = payload.Value<string>("character_id");

            var isOutfitTracked = _outfitMap.TryGetValue(characterId, out string alias);

            if (!isOutfitTracked)
            {
                return;
            }

            if (!_membersOnlineMap[alias].Any(m => m.Id == characterId))
            {
                var character = _membersMap[alias].FirstOrDefault(m => m.Id == characterId);
                if (character == null)
                {
                    return;
                }

                _membersOnlineMap[alias].Add(character);
            }
        }

        private void HandlePlayerLogoutPayload(JToken payload)
        {
            var characterId = payload.Value<string>("character_id");

            var isOutfitTracked = _outfitMap.TryGetValue(characterId, out string alias);

            if (!isOutfitTracked)
            {
                return;
            }

            if (_membersOnlineMap[alias].Any(m => m.Id == characterId))
            {
                var character = _membersMap[alias].FirstOrDefault(m => m.Id == characterId);
                if (character == null)
                {
                    return;
                }

                _membersOnlineMap[alias].Remove(character);
            }
        }

        private void SubscribeToCensus()
        {
            foreach (var alias in _validOutfitAliasHistory)
            {
                if (_isMembersLoading.Contains(alias) || _subscribedOutfitsHistory.Contains(alias))
                {
                    continue;
                }

                //_wsMonitor.AddCharacterSubscriptions(_membersMap[alias].Select(m => m.Id).ToList());
                _subscribedOutfitsHistory.Add(alias);
            }
        }

        private async void HandleValidOutfitAliasSubmit(OutfitAlias inputAlias)
        {
            Outfit newOutfit;

            if (!_validOutfitAliasHistory.Contains(inputAlias.Alias))
            {
                var newAlias = inputAlias.Alias;

                _isAliasLoading.Add(newAlias);

                newOutfit = await _outfits.GetOutfitByAlias(newAlias);

                if (newOutfit != null)
                {
                    _teamOutfits1.Add(newOutfit);
                    _validOutfitAliasHistory.Add(newAlias); // _inputAlias1.Alias);
                    inputAlias.Alias = string.Empty;

                    _isAliasLoading.Remove(newAlias);

                    await GetOutfitMembers(newAlias);
                }
            }
        }

        private async Task GetOutfitMembers(string alias)
        {
            _isMembersLoading.Add(alias);

            var censusMembers = await _outfits.GetOutfitMembersByAlias(alias);

            if (censusMembers != null && censusMembers.Any())
            {
                _membersMap.Add(alias, censusMembers.ToList());

                var onlineMembers = censusMembers.Where(m => m.IsOnline == true);

                _membersOnlineMap.Add(alias, onlineMembers.ToList());

                foreach (var member in censusMembers)
                {
                    _outfitMap.Add(member.Id, alias);
                }

                _isMembersLoading.Remove(alias);
            }
        }

        
    }
}
