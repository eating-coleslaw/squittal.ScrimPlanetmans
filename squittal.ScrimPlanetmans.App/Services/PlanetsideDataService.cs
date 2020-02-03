using squittal.ScrimPlanetmans.CensusStream;
using squittal.ScrimPlanetmans.Services.Planetside;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services
{
    public class PlanetsideDataService
    {
        private readonly IOutfitService _outfits;
        private readonly ICharacterService _characters;
        //private readonly IWebsocketMonitor _wsMonitor;

        public PlanetsideDataService(IOutfitService outfits, ICharacterService characters) //, IWebsocketMonitor wsMonitor)
        {
            _outfits = outfits;
            _characters = characters;
            //_wsMonitor = wsMonitor;
        }

        public async Task<Outfit> GetOutfitByAlias(string alias)
        {
            return await _outfits.GetOutfitByAlias(alias);
        }

        public async Task<Character> GetCharacterById(string id)
        {
            return await _characters.GetCharacterAsync(id);
        }

        public async Task<OutfitMember> GetCharacterOutfit(string id)
        {
            return await _characters.GetCharacterOutfitAsync(id);
        }

        public async Task<IEnumerable<Character>> GetOutfitMembersByAlias(string alias)
        {
            return await _outfits.GetOutfitMembersByAlias(alias);
        }

        //public void AddCharactersDeathSubscription(IEnumerable<string> characterIds)
        //{
        //    /*
        //    _wsMonitor.SubscriptionBuilder.AddDeath();
        //    _wsMonitor.SubscriptionBuilder.AddCharactersAll(); // characterIds);
        //    //_wsMonitor.SubscriptionBuilder.AddWorldsAll();
        //    */
        //    throw new NotImplementedException();
        //}

        //public void NewCharacterDeathSubscription(string characterId)
        //{
        //    /*
        //    _wsMonitor.SubscriptionBuilder.ClearSubscriptions();
        //    _wsMonitor.SubscriptionBuilder.AddCharacter(characterId);
        //    _wsMonitor.SubscriptionBuilder.AddDeath();
        //    */
        //    throw new NotImplementedException();
        //}
    }
}
