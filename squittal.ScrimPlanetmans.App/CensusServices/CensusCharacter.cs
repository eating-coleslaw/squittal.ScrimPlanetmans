using DaybreakGames.Census;
using squittal.ScrimPlanetmans.CensusServices.Models;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.CensusServices
{
    public class CensusCharacter
    {
        private readonly ICensusQueryFactory _queryFactory;

        public CensusCharacter(ICensusQueryFactory queryFactory)
        {
            _queryFactory = queryFactory;
        }

        public async Task<CensusCharacterModel> GetCharacter(string characterId)
        {
            var query = _queryFactory.Create("character");
            query.AddResolve("world");
            query.ShowFields("character_id", "name.first", "faction_id", "world_id", "battle_rank.value", "battle_rank.percent_to_next", "certs.earned_points", "title_id", "prestige_level");
            query.Where("character_id").Equals(characterId);

            return await query.GetAsync<CensusCharacterModel>();
        }

        public async Task<CensusCharacterModel> GetCharacterByName(string characterName)
        {
            var query = _queryFactory.Create("character");
            query.AddResolve("world");
            query.ShowFields("character_id", "name.first", "faction_id", "world_id", "battle_rank.value", "battle_rank.percent_to_next", "certs.earned_points", "title_id", "prestige_level");
            query.Where("name.first_lower").Equals(characterName.ToLower());

            return await query.GetAsync<CensusCharacterModel>();
        }

        public async Task<string> GetCharacterIdByName(string characterName)
        {
            var query = _queryFactory.Create("character_name");

            query.Where("name.first_lower").Equals(characterName.ToLower());

            var result = await query.GetAsync<CensusCharacterModel>();

            return result?.CharacterId;
        }

        public async Task<CensusOutfitMemberModel> GetCharacterOutfitMembership(string characterId)
        {
            var query = _queryFactory.Create("outfit_member");

            query.ShowFields("character_id", "outfit_id", "member_since_date", "rank", "rank_ordinal");
            query.Where("character_id").Equals(characterId);

            return await query.GetAsync<CensusOutfitMemberModel>();
        }

        public async Task<CensusCharacterModel.CharacterTimes> GetCharacterTimes(string characterId)
        {
            var query = _queryFactory.Create("character");

            query.ShowFields("character_id", "times.creation_date", "times.last_save_date", "times.last_login_date", "times.minutes_played");
            query.Where("character_id").Equals(characterId);

            var result = await query.GetAsync<CensusCharacterModel>();

            return result?.Times;
        }
    }
}
