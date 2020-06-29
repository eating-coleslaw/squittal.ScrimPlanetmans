using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class VehicleClassConfiguration : IEntityTypeConfiguration<VehicleClass>
    {
        public void Configure(EntityTypeBuilder<VehicleClass> builder)
        {
            builder.ToTable("VehicleClass");

            builder.HasKey(e => e.Class);
        }
    }
}
