using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.Planetside;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class MapRegionConfiguration : IEntityTypeConfiguration<MapRegion>
    {
        public void Configure(EntityTypeBuilder<MapRegion> builder)
        {
            builder.ToTable("MapRegion");

            builder.HasKey(e => new { e.Id, e.FacilityId });

            builder.Property(e => e.Id).ValueGeneratedNever();
        }
    }
}
