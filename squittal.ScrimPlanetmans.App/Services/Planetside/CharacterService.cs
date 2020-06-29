using System.Threading.Tasks;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Models.Planetside;

//using System.Linq;
//using Microsoft.EntityFrameworkCore;
//using squittal.ScrimPlanetmans.Data;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class CharacterService : ICharacterService
    {
        //private readonly IDbContextHelper _dbContextHelper;
        private readonly IOutfitService _outfitService;
        private readonly CensusCharacter _censusCharacter;

        public CharacterService(/*IDbContextHelper dbContextHelper,*/ IOutfitService outfitService, CensusCharacter censusCharacter)
        {
            //_dbContextHelper = dbContextHelper;
            _outfitService = outfitService;
            _censusCharacter = censusCharacter;
        }

        public async Task<Character> GetCharacterAsync(string characterId)
        {
            try
            {
                var character = await _censusCharacter.GetCharacter(characterId);
            
                if (character == null)
                {
                    return null;
                }
            
                var censusEntity = ConvertToDbModel(character);
            
                return censusEntity;
            }
            catch
            {
                // TODO: add logging to this service
                return null;
            }

            /*

            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                //IQueryable<Character> characterQuery = dbContext.Characters.Where(e => e.Id == characterId);

                var storeCharacter =  await dbContext.Characters
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(e => e.Id == characterId);

                if (storeCharacter != null)
                {
                    return storeCharacter;
                }

                var character = await _censusCharacter.GetCharacter(characterId);

                if (character == null)
                {
                    return null;
                }

                var censusEntity = ConvertToDbModel(character);

                dbContext.Add(censusEntity);
                await dbContext.SaveChangesAsync();

                return censusEntity;
            }
            */
        }

        public async Task<Character> GetCharacterByNameAsync(string characterName)
        {
            var character = await _censusCharacter.GetCharacterByName(characterName);

            if (character == null)
            {
                return null;
            }

            var censusEntity = ConvertToDbModel(character);

            return censusEntity;
        }

        public async Task<OutfitMember> GetCharacterOutfitAsync(string characterId)
        {
            var character = await GetCharacterAsync(characterId);
            if (character == null)
            {
                return null;
            }

            return await _outfitService.UpdateCharacterOutfitMembership(character);
        }

        public static Character ConvertToDbModel(CensusCharacterModel censusModel)
        {
            bool isOnline;
            
            if (int.TryParse(censusModel.OnlineStatus, out int onlineStatus))
            {
                isOnline = onlineStatus > 0 ? true : false;
            }
            else // "service_unavailable"
            {
                isOnline = false;
            }
            //bool isOnline = censusModel.OnlineStatus > 0 ? true : false;

            return new Character
            {
                Id = censusModel.CharacterId,
                Name = censusModel.Name.First,
                FactionId = censusModel.FactionId,
                TitleId = censusModel.TitleId,
                WorldId = censusModel.WorldId,
                BattleRank = censusModel.BattleRank.Value,
                BattleRankPercentToNext = censusModel.BattleRank.PercentToNext,
                CertsEarned = censusModel.Certs.EarnedPoints,
                PrestigeLevel = censusModel.PrestigeLevel,
                IsOnline = isOnline
            };
        }
    }
}
