using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using squittal.ScrimPlanetmans.ScrimMatch.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class RulesetConfiguration : IEntityTypeConfiguration<Ruleset>
    {
        public void Configure(EntityTypeBuilder<Ruleset> builder)
        {
            builder.ToTable("Ruleset");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.DefaultMatchTitle).HasDefaultValue(null);
            builder.Property(e => e.SourceFile).HasDefaultValue(string.Empty);
            builder.Property(e => e.IsCustomDefault).HasDefaultValue(false);
            builder.Property(e => e.IsDefault).HasDefaultValue(false);

            //builder.Property(e => e.EnableRoundTimeLimit).HasDefaultValue(true);
            builder.Property(e => e.DefaultRoundLength).HasDefaultValue(900);
            //builder.Property(e => e.RoundTimerDirection).HasDefaultValue(null);

            builder.Property(e => e.DefaultEndRoundOnFacilityCapture).HasDefaultValue(false);
     
            builder.Property(e => e.EndRoundOnPointValueReached).HasDefaultValue(false);
            //builder.Property(e => e.TargetPointValue).HasDefaultValue(null);
            //builder.Property(e => e.InitialPoints).HasDefaultValue(null);
            
            builder.Property(e => e.MatchWinCondition).HasDefaultValue(MatchWinCondition.MostPoints);
            builder.Property(e => e.RoundWinCondition).HasDefaultValue(RoundWinCondition.NotApplicable);
            
            builder.Property(e => e.EnablePeriodicFacilityControlRewards).HasDefaultValue(false);
            //builder.Property(e => e.PeriodicFacilityControlPoints).HasDefaultValue(null);
            //builder.Property(e => e.PeriodicFacilityControlInterval).HasDefaultValue(null);
            //builder.Property(e => e.PeriodFacilityControlPointAttributionType).HasDefaultValue((PointAttributionType?)null);
        }
    }
}
