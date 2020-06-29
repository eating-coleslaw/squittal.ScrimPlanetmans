using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusOutfit
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusOutfit(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<CensusOutfitModel> GetOutfitAsync(string outfitId)
        {
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "alias_lower", "time_created", "leader_character_id", "member_count");

            query.Where("outfit_id").Equals(outfitId);

            return await query.GetAsync<CensusOutfitModel>();
        }

        public async Task<CensusOutfitModel> GetOutfitByAliasAsync(string alias)
        {
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "alias_lower", "time_created", "leader_character_id", "member_count");

            query.Where("alias_lower").Equals(alias.ToLower());

            return await query.GetAsync<CensusOutfitModel>();
        }

        public async Task<IEnumerable<CensusOutfitMemberCharacterModel>> GetOutfitMembersAsync(string outfitId)
        { 
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "alias_lower", "member_count");

            query.Where("outfit_id").Equals(outfitId);

            //query.AddResolve("member_character_name");
            query.AddResolve("member_character(name,prestige_level)");
            query.AddResolve("member_online_status");

            var result = await query.GetAsync<CensusOutfitResolveMemberCharacterModel>();

            return result.Members;
        }

        public async Task<IEnumerable<CensusOutfitMemberCharacterModel>> GetOutfitMembersByAliasAsync(string alias)
        {
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "alias_lower", "member_count");

            query.Where("alias_lower").Equals(alias.ToLower());

            //query.AddResolve("member_character_name");
            query.AddResolve("member_character(name,prestige_level)");
            query.AddResolve("member_online_status");

            var result = await query.GetAsync<CensusOutfitResolveMemberCharacterModel>();

            var outfit = ConvertToCensusModel(result);

            var members = result.Members.Select(m => FillOutOutfitMemberCharacterModel(m, outfit)).ToList();

            return members;
        }

        private CensusOutfitModel ConvertToCensusModel(CensusOutfitResolveMemberCharacterModel result)
        {
            return new CensusOutfitModel
            {
                OutfitId = result.OutfitId,
                Name = result.Name,
                Alias = result.Alias,
                AliasLower = result.AliasLower,
                MemberCount = result.MemberCount
            };
        }

        private CensusOutfitMemberCharacterModel FillOutOutfitMemberCharacterModel(CensusOutfitMemberCharacterModel character, CensusOutfitModel outfit)
        {
            return new CensusOutfitMemberCharacterModel
            {
                OutfitId = outfit.OutfitId,
                CharacterId = character.CharacterId,
                Name = character.Name,
                OnlineStatus = character.OnlineStatus,
                OutfitAlias = outfit.Alias,
                OutfitAliasLower = outfit.AliasLower,
                PrestigeLevel = character.PrestigeLevel
            };
        }
    }
}
