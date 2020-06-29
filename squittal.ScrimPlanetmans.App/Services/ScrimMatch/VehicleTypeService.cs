using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using squittal.ScrimPlanetmans.Data;
using squittal.ScrimPlanetmans.ScrimMatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace squittal.ScrimPlanetmans.Services.ScrimMatch
{
    public class VehicleTypeService : IVehicleTypeService
    {
        private readonly IDbContextHelper _dbContextHelper;
        public ILogger<VehicleTypeService> _logger;

        public VehicleTypeService(IDbContextHelper dbContextHelper, ILogger<VehicleTypeService> logger)
        {
            _dbContextHelper = dbContextHelper;
            _logger = logger;
        }

        public async Task SeedVehicleClasses()
        {
            using var factory = _dbContextHelper.GetFactory();
            var dbContext = factory.GetDbContext();

            var createdEntities = new List<VehicleClass>();

            var allVehicleTypeValues = new List<VehicleType>();

            //var enumValues = (VehicleType[])Enum.GetValues(typeof(VehicleType));
            var enumValues = GetVehicleTypes();

            allVehicleTypeValues.AddRange(enumValues);

            var storeEntities = await dbContext.VehicleClasses.ToListAsync();

            allVehicleTypeValues.AddRange(storeEntities.Where(a => !allVehicleTypeValues.Contains(a.Class)).Select(a => a.Class).ToList());

            allVehicleTypeValues.Distinct().ToList();

            foreach (var value in allVehicleTypeValues)
            {
                try
                {

                    var storeEntity = storeEntities.FirstOrDefault(e => e.Class == value);
                    var isValidEnum = enumValues.Any(enumValue => enumValue == value);

                    if (storeEntity == null)
                    {
                        createdEntities.Add(ConvertToDbModel(value));
                    }
                    else if (isValidEnum)
                    {
                        storeEntity = ConvertToDbModel(value);
                        dbContext.VehicleClasses.Update(storeEntity);
                    }
                    else
                    {
                        dbContext.VehicleClasses.Remove(storeEntity);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            if (createdEntities.Any())
            {
                await dbContext.VehicleClasses.AddRangeAsync(createdEntities);
            }

            await dbContext.SaveChangesAsync();

            _logger.LogInformation($"Seeded Vehicle Classes store");
        }

        private VehicleClass ConvertToDbModel(VehicleType value)
        {
            var name = Enum.GetName(typeof(VehicleType), value);

            string description;

            if (value == VehicleType.MBT)
            {
                description = "Main Battle Tank";
            }
            else if (value == VehicleType.ESF)
            {
                description = "Empire-Specific Fighter";
            }
            else
            {
                description = Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1 $2");
            }

            return new VehicleClass
            {
                Class = value,
                Name = name,
                Description = description // Regex.Replace(name, @"(\p{Ll})(\p{Lu})", "$1 $2")
            };
        }

        public IEnumerable<VehicleType> GetVehicleTypes()
        {
            return (VehicleType[])Enum.GetValues(typeof(VehicleType));
        }
    }
}
