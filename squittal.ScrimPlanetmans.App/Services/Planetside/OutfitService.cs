using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DaybreakGames.Census.Exceptions;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.CensusServices;
using squittal.ScrimPlanetmans.CensusServices.Models;
using squittal.ScrimPlanetmans.Shared.Models.Planetside;

//using Microsoft.EntityFrameworkCore;
//using squittal.ScrimPlanetmans.Data;

namespace squittal.ScrimPlanetmans.Services.Planetside
{
    public class OutfitService : IOutfitService
    {
        //private readonly IDbContextHelper _dbContextHelper;
        private readonly CensusOutfit _censusOutfit;
        private readonly CensusCharacter _censusCharacter;
        private readonly ILogger<OutfitService> _logger;

        public OutfitService(/*IDbContextHelper dbContextHelper,*/ CensusOutfit censusOutfit, CensusCharacter censusCharacter, ILogger<OutfitService> logger)
        {
            //_dbContextHelper = dbContextHelper;
            _censusOutfit = censusOutfit;
            _censusCharacter = censusCharacter;
            _logger = logger;
        }

        public async Task<Outfit> GetOutfitAsync(string outfitId)
        {
            return await GetOutfitInternalAsync(outfitId);
        }

        public async Task<Outfit> GetOutfitByAlias(string alias)
        {
            var outfit = await _censusOutfit.GetOutfitByAliasAsync(alias);
            if (outfit == null)
            {
                return null;
            }

            var censusEntity = ConvertToDbModel(outfit);
            var resolvedOutfit = await ResolveOutfitDetailsAsync(censusEntity, null);

            return resolvedOutfit;

            /*
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Outfits.AsNoTracking().FirstOrDefaultAsync(o => o.Alias.ToLower() == alias.ToLower());
            }
            */
        }

        public async Task<IEnumerable<Character>> GetOutfitMembersByAlias(string alias)
        {
            var members = await _censusOutfit.GetOutfitMembersByAliasAsync(alias);
            if (members == null)
            {
                return null;
            }

            var validMembers = members.Where(m => m.CharacterId != null && m.Name != null);

            var censusEntities = validMembers.Select(ConvertToDbModel);

            return censusEntities.ToList();
        }

        public async Task<OutfitMember> UpdateCharacterOutfitMembership(Character character)
        {
            OutfitMember outfitMember;
            CensusOutfitMemberModel membership;
            
            try
            {
                membership = await _censusCharacter.GetCharacterOutfitMembership(character.Id);
            }
            catch (CensusConnectionException)
            {
                return null;
            }

            if (membership == null)
            {
                //await RemoveOutfitMemberAsync(character.Id);
                return null;
            }

            var outfit = await GetOutfitInternalAsync(membership.OutfitId, character);
            if (outfit == null)
            {
                _logger.LogError(84624, $"Unable to resolve outfit {membership.OutfitId} for character {character.Id}");
                return null;
            }

            outfitMember = new OutfitMember
            {
                OutfitId = membership.OutfitId,
                CharacterId = membership.CharacterId,
                FactionId = character.FactionId,
                MemberSinceDate = membership.MemberSinceDate,
                Rank = membership.Rank,
                RankOrdinal = membership.RankOrdinal
            };

            //outfitMember = await UpsertOutfitMemberAsync(outfitMember);
            return outfitMember;
        }
        
        private async Task<Outfit> GetOutfitInternalAsync(string outfitId, Character member = null)
        {
            Outfit outfit;
            
            outfit = await GetKnownOutfitAsync(outfitId);

            if (outfit == null)
            {
                return null;
            }

            // These are null if outfit was retrieved from the census API
            if (outfit.WorldId == null || outfit.FactionId == null)
            {
                outfit = await ResolveOutfitDetailsAsync(outfit, member);
                //await UpsertOutfitAsync(outfit);
            }

            return outfit;
        }

        // Returns the specified outfit from the database, if it exists. Otherwise,
        // queries for it in the DBG census.
        private async Task<Outfit> GetKnownOutfitAsync(string outfitId)
        {
            Outfit outfit;

            try
            {
                outfit = await GetCensusOutfit(outfitId);
            }
            catch (CensusConnectionException)
            {
                return null;
            }

            return outfit;

            /*
            outfit = await GetDbOutfitAsync(outfitId);

            if (outfit == null)
            {
                try
                {
                    outfit = await GetCensusOutfit(outfitId);
                }
                catch (CensusConnectionException)
                {
                    return null;
                }
            }

            return outfit;
            */
        }

        /*
        private async Task<Outfit> GetDbOutfitAsync(string outfitId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                return await dbContext.Outfits.AsNoTracking().FirstOrDefaultAsync(o => o.Id == outfitId);
            }
        }
        */

        private async Task<Outfit> GetCensusOutfit(string outfitId)
        {
            var censusOutfit = await _censusOutfit.GetOutfitAsync(outfitId);

            return censusOutfit == null
                ? null
                : ConvertToDbModel(censusOutfit);
            //{
            //    Id = censusOutfit.OutfitId,
            //    Alias = censusOutfit.Alias,
            //    AliasLower = censusOutfit.AliasLower,
            //    Name = censusOutfit.Name,
            //    LeaderCharacterId = censusOutfit.LeaderCharacterId,
            //    CreatedDate = censusOutfit.TimeCreated,
            //    MemberCount = censusOutfit.MemberCount
            //};
        }

        public static Outfit ConvertToDbModel(CensusOutfitModel censusOutfit)
        {
            return new Outfit
            {
                Id = censusOutfit.OutfitId,
                Alias = censusOutfit.Alias,
                AliasLower = censusOutfit.AliasLower,
                Name = censusOutfit.Name,
                LeaderCharacterId = censusOutfit.LeaderCharacterId,
                CreatedDate = censusOutfit.TimeCreated,
                MemberCount = censusOutfit.MemberCount
            };
        }

        private async Task<Outfit> ResolveOutfitDetailsAsync(Outfit outfit, Character member)
        {
            if (member != null)
            {
                outfit.WorldId = member.WorldId;
                outfit.FactionId = member.FactionId;
            }
            else
            {
                var leader = await _censusCharacter.GetCharacter(outfit.LeaderCharacterId);
                outfit.WorldId = leader.WorldId;
                outfit.FactionId = leader.FactionId;
            }

            return outfit;
        }

        public static Character ConvertToDbModel(CensusOutfitMemberCharacterModel member)
        {
            bool isOnline = member.OnlineStatus > 0 ? true : false;

            return new Character
            {
                Id = member.CharacterId,
                Name = member.Name.First,
                IsOnline = isOnline,
                OutfitId = member.OutfitId,
                OutfitAlias = member.OutfitAlias,
                OutfitAliasLower = member.OutfitAliasLower
            };
        }

        /*
        private async Task<Outfit> UpsertOutfitAsync(Outfit newEntity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storeEntity = await dbContext.Outfits.AsNoTracking().FirstOrDefaultAsync(o => o.Id == newEntity.Id);

                if (storeEntity == null)
                {
                    await dbContext.AddAsync(newEntity);
                }
                else
                {
                    storeEntity = newEntity;
                    dbContext.Update(storeEntity);
                }
                await dbContext.SaveChangesAsync();
            }
            return newEntity;
        }
        */

        /*
        private async Task<OutfitMember> UpsertOutfitMemberAsync(OutfitMember newEntity)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storeEntity = await dbContext.OutfitMembers.AsNoTracking().FirstOrDefaultAsync(m => m.CharacterId == newEntity.CharacterId);

                if (storeEntity == null)
                {
                    await dbContext.AddAsync(newEntity);
                }
                else
                {
                    storeEntity = newEntity;
                    dbContext.Update(storeEntity);
                }
                await dbContext.SaveChangesAsync();
            }
            return newEntity;
        }
        */

        /*
        private async Task RemoveOutfitMemberAsync(string characterId)
        {
            using (var factory = _dbContextHelper.GetFactory())
            {
                var dbContext = factory.GetDbContext();

                var storeMembership = await dbContext.OutfitMembers.FindAsync(characterId);

                if (storeMembership != null)
                {
                    dbContext.OutfitMembers.Remove(storeMembership);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
        */
    }
}
