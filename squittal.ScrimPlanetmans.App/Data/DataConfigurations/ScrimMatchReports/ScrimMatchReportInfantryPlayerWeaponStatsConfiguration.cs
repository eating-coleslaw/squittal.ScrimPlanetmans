using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchReportInfantryPlayerWeaponStatsConfiguration : IEntityTypeConfiguration<ScrimMatchReportInfantryPlayerWeaponStats>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchReportInfantryPlayerWeaponStats> builder)
        {
            builder.ToView("View_ScrimMatchReportInfantryPlayerWeaponStats");

            builder.HasNoKey();

            builder.Ignore(p => p.HeadshotKillPercent);
            builder.Ignore(p => p.HeadshotDeathPercent);

            builder.Property(e => e.WeaponFactionId).HasDefaultValue(4);

            builder.Property(e => e.PrestigeLevel).HasDefaultValue(0);
            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.NetScore).HasDefaultValue(0);
            builder.Property(e => e.Teamkills).HasDefaultValue(0);
            builder.Property(e => e.TeamkillDeaths).HasDefaultValue(0);
            builder.Property(e => e.Suicides).HasDefaultValue(0);
            
            builder.Property(e => e.Kills).HasDefaultValue(0);
            builder.Property(e => e.Deaths).HasDefaultValue(0);
            builder.Property(e => e.HeadshotKills).HasDefaultValue(0);
            builder.Property(e => e.HeadshotDeaths).HasDefaultValue(0);
            builder.Property(e => e.ScoredDeaths).HasDefaultValue(0);
            builder.Property(e => e.ZeroPointDeaths).HasDefaultValue(0);
            builder.Property(e => e.ScoredKills).HasDefaultValue(0);
            builder.Property(e => e.ZeroPointKills).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedKills).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.AssistedKills).HasDefaultValue(0);
            builder.Property(e => e.UnassistedKills).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedKills).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.AssistedKills).HasDefaultValue(0);
            builder.Property(e => e.UnassistedKills).HasDefaultValue(0);

            builder.Property(e => e.HeadshotTeamkills).HasDefaultValue(0);
            builder.Property(e => e.EnemyDeaths).HasDefaultValue(0);
            builder.Property(e => e.HeadshotEnemyDeaths).HasDefaultValue(0);
            builder.Property(e => e.ScoredEnemyDeaths).HasDefaultValue(0);
            builder.Property(e => e.ZeroPointEnemyDeaths).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedEnemyDeaths).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedEnemyDeaths).HasDefaultValue(0);
        }
    }
}
