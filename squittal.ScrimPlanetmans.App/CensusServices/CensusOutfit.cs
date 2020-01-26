using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            query.ShowFields("outfit_id", "name", "alias", "time_created", "leader_character_id", "member_count");

            query.Where("outfit_id").Equals(outfitId);

            return await query.GetAsync<CensusOutfitModel>();
        }

        public async Task<CensusOutfitModel> GetOutfitByAliasAsync(string alias)
        {
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "time_created", "leader_character_id", "member_count");

            query.Where("alias").Equals(alias);

            return await query.GetAsync<CensusOutfitModel>();
        }

        public async Task<IEnumerable<CensusOutfitMemberCharacterNameModel>> GetOutfitMembersAsync(string outfitId)
        { 
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "member_count");

            query.Where("outfit_id").Equals(outfitId);

            query.AddResolve("member_character_name");

            var result = await query.GetAsync<CensusOutfitResolveMemberCharacterNameModel>();

            return result.Members;
        }

        public async Task<IEnumerable<CensusOutfitMemberCharacterNameModel>> GetOutfitMembersByAliasAsync(string alias)
        {
            var query = _queryFactory.Create("outfit");

            query.ShowFields("outfit_id", "name", "alias", "member_count");

            query.Where("alias").Equals(alias);

            query.AddResolve("member_character_name");

            var result = await query.GetAsync<CensusOutfitResolveMemberCharacterNameModel>();

            return result.Members;
        }
    }
}
