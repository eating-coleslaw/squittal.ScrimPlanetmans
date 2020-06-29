using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchRoundConfigurationConfiguration : IEntityTypeConfiguration<ScrimMatchRoundConfiguration>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchRoundConfiguration> builder)
        {
            builder.ToTable("ScrimMatchRoundConfiguration");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.ScrimMatchRound
            });

            builder.Property(e => e.IsManualWorldId).HasDefaultValue(false);
            builder.Property(e => e.IsRoundEndedOnFacilityCapture).HasDefaultValue(false);

            builder.Ignore(e => e.ScrimMatch);
            builder.Ignore(e => e.World);
        }
    }
}