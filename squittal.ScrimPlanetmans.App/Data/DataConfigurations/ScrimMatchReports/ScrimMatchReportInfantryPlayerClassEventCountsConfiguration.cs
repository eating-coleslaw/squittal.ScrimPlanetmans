using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchReportInfantryPlayerClassEventCountsConfiguration : IEntityTypeConfiguration<ScrimMatchReportInfantryPlayerClassEventCounts>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchReportInfantryPlayerClassEventCounts> builder)
        {
            builder.ToView("View_ScrimMatchReportInfantryPlayerClassEventCounts");

            builder.HasNoKey();

            builder.Ignore(e => e.PrimaryPlanetsideClass);

            builder.Property(e => e.PrestigeLevel).HasDefaultValue(0);
            builder.Property(e => e.EventsAsHeavyAssault).HasDefaultValue(0);
            builder.Property(e => e.EventsAsInfiltrator).HasDefaultValue(0);
            builder.Property(e => e.EventsAsLightAssault).HasDefaultValue(0);
            builder.Property(e => e.EventsAsMedic).HasDefaultValue(0);
            builder.Property(e => e.EventsAsEngineer).HasDefaultValue(0);
            builder.Property(e => e.EventsAsMax).HasDefaultValue(0);
            builder.Property(e => e.KillsAsHeavyAssault).HasDefaultValue(0);
            builder.Property(e => e.KillsAsInfiltrator).HasDefaultValue(0);
            builder.Property(e => e.KillsAsLightAssault).HasDefaultValue(0);
            builder.Property(e => e.KillsAsMedic).HasDefaultValue(0);
            builder.Property(e => e.KillsAsEngineer).HasDefaultValue(0);
            builder.Property(e => e.KillsAsMax).HasDefaultValue(0);
            builder.Property(e => e.DeathsAsHeavyAssault).HasDefaultValue(0);
            builder.Property(e => e.DeathsAsInfiltrator).HasDefaultValue(0);
            builder.Property(e => e.DeathsAsLightAssault).HasDefaultValue(0);
            builder.Property(e => e.DeathsAsMedic).HasDefaultValue(0);
            builder.Property(e => e.DeathsAsEngineer).HasDefaultValue(0);
            builder.Property(e => e.DeathsAsMax).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsHeavyAssault).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsInfiltrator).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsLightAssault).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsMedic).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsEngineer).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsMax).HasDefaultValue(0);
        }
    }
}
