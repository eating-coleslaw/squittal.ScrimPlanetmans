using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class DeathEventTypeService : IDeathEventTypeService
    {
        private readonly IDbContextHelper _dbContextHelper;
        public ILogger<DeathEventTypeService> _logger;

        public DeathEventTypeService(IDbContextHelper dbContextHelper, ILogger<DeathEventTypeService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }

        public async Task SeedDeathTypes()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var createdEntities = new List<DeathType>();

            var allDeathEventTypeValues = new List<DeathEventType>();

            var enumValues = GetDeathEventTypes();

            allDeathEventTypeValues.AddRange(enumValues);

            var storeEntities = await dbContext.DeathTypes.ToListAsync();

            allDeathEventTypeValues.AddRange(storeEntities.Where(a => !allDeathEventTypeValues.Contains(a.Type)).Select(a => a.Type).ToList());

            allDeathEventTypeValues.Distinct().ToList();

            foreach (var value in allDeathEventTypeValues)
            {
                try
                {

                    var storeEntity = storeEntities.FirstOrDefault(e => e.Type == value);
                    var isValidEnum = enumValues.Any(enumValue => enumValue == value);

                    if (storeEntity == null)
                    {
                        createdEntities.Add(ConvertToDbModel(value));
                    }
                    else if (isValidEnum)
                    {
                        storeEntity = ConvertToDbModel(value);
                        dbContext.DeathTypes.Update(storeEntity);
                    }
                    else
                    {
                        dbContext.DeathTypes.Remove(storeEntity);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            if (createdEntities.Any())
            {
                await dbContext.DeathTypes.AddRangeAsync(createdEntities);
            }

            await dbContext.SaveChangesAsync();

            _logger.LogInformation($"Seeded Death Types store");
        }

        private DeathType ConvertToDbModel(DeathEventType value)
        {
            var name = Enum.GetName(typeof(DeathEventType), value);

            return new DeathType
            {
                Type = value,
                Name = name
            };
        }

        public IEnumerable<DeathEventType> GetDeathEventTypes()
        {
            return (DeathEventType[])Enum.GetValues(typeof(DeathEventType));
        }
    }
}
