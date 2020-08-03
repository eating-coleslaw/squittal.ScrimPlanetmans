using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchReportInfantryPlayerStatsConfiguration : IEntityTypeConfiguration<ScrimMatchReportInfantryPlayerStats>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchReportInfantryPlayerStats> builder)
        {
            builder.ToView("View_ScrimMatchReportInfantryPlayerStats");

            builder.HasNoKey();

            builder.Ignore(p => p.OneVsOneCount);
            builder.Ignore(p => p.OneVsOneRatio);
            builder.Ignore(p => p.UnassistedKills);

            builder.Property(e => e.PrestigeLevel).HasDefaultValue(0);
            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.NetScore).HasDefaultValue(0);
            builder.Property(e => e.Kills).HasDefaultValue(0);
            builder.Property(e => e.Deaths).HasDefaultValue(0);
            builder.Property(e => e.TeamKills).HasDefaultValue(0);
            builder.Property(e => e.Suicides).HasDefaultValue(0);
            builder.Property(e => e.ScoredDeaths).HasDefaultValue(0);
            builder.Property(e => e.ZeroPointDeaths).HasDefaultValue(0);
            builder.Property(e => e.TeamKillDeaths).HasDefaultValue(0);
            builder.Property(e => e.DamageAssists).HasDefaultValue(0);
            builder.Property(e => e.DamageTeamAssists).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedKills).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedEnemyDeaths).HasDefaultValue(0);
            builder.Property(e => e.UnassistedEnemyDeaths).HasDefaultValue(0);
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
        }
    }
}
