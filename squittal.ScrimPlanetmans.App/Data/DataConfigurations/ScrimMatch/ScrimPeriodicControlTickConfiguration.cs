using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimPeriodicControlTickConfiguration : IEntityTypeConfiguration<ScrimPeriodicControlTick>
    {
        public void Configure(EntityTypeBuilder<ScrimPeriodicControlTick> builder)
        {
            builder.ToTable("ScrimPeriodicControlTick");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.Timestamp
            });

            builder.Property(e => e.Points).HasDefaultValue(0);

            builder.Ignore(e => e.ScrimMatch);
        }
    }
}