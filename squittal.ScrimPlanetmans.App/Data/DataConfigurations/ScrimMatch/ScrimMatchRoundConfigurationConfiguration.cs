using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.Data.Models;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

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
            
            builder.Property(e => e.TargetPointValue).HasDefaultValue(null);
            builder.Property(e => e.InitialPoints).HasDefaultValue(null);

            builder.Property(e => e.PeriodicFacilityControlPoints).HasDefaultValue(null);
            builder.Property(e => e.PeriodicFacilityControlInterval).HasDefaultValue(null);
            
            builder.Property(e => e.EnableRoundTimeLimit).HasDefaultValue(true);
            //builder.Property(e => e.RoundTimerDirection).HasDefaultValue(null);
            builder.Property(e => e.EndRoundOnPointValueReached).HasDefaultValue(false);
            builder.Property(e => e.MatchWinCondition).HasDefaultValue(MatchWinCondition.MostPoints);
            builder.Property(e => e.RoundWinCondition).HasDefaultValue(RoundWinCondition.NotApplicable);
            builder.Property(e => e.EnablePeriodicFacilityControlRewards).HasDefaultValue(false);

            builder.Ignore(e => e.ScrimMatch);
            builder.Ignore(e => e.World);
        }
    }
}