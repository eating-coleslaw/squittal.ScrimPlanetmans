using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchReportInfantryPlayerHeadToHeadStatsConfiguration : IEntityTypeConfiguration<ScrimMatchReportInfantryPlayerHeadToHeadStats>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchReportInfantryPlayerHeadToHeadStats> builder)
        {
            builder.ToView("View_ScrimMatchReportInfantryPlayerHeadToHeadStats");

            builder.HasNoKey();

            builder.Ignore(p => p.WeightedAssistedDeaths);
            builder.Ignore(p => p.KillDeathEngagementCount);
            builder.Ignore(p => p.EngagementCount);
            builder.Ignore(p => p.WeightedEngagementCount);
            builder.Ignore(p => p.FavorableEngagementCount);
            builder.Ignore(p => p.WeightedFavorableEngagementPercent);
            builder.Ignore(p => p.OneVsOneCount);
            builder.Ignore(p => p.OneVsOneEngagementPercent);
            builder.Ignore(p => p.OneVsOneKillDeathRatio);
            builder.Ignore(p => p.HeadshotKillPercent);
            builder.Ignore(p => p.HeadshotDeathPercent);
            builder.Ignore(p => p.OneVsOneHeadshotKillRatio);
            builder.Ignore(p => p.OneVsOneHeadshotDeathRatio);

            builder.Property(e => e.PlayerPrestigeLevel).HasDefaultValue(0);
            builder.Property(e => e.OpponentPrestigeLevel).HasDefaultValue(0);
            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.NetScore).HasDefaultValue(0);
            builder.Property(e => e.Kills).HasDefaultValue(0);
            builder.Property(e => e.HeadshotKills).HasDefaultValue(0);
            builder.Property(e => e.Deaths).HasDefaultValue(0);
            builder.Property(e => e.HeadshotDeaths).HasDefaultValue(0);
            builder.Property(e => e.ScoredDeaths).HasDefaultValue(0);
            builder.Property(e => e.ZeroPointDeaths).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsDealt).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsTaken).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedKills).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedOnlyKills).HasDefaultValue(0);
            builder.Property(e => e.GrenadeAssistedKills).HasDefaultValue(0);
            builder.Property(e => e.GrenadeAssistedOnlyKills).HasDefaultValue(0);
            builder.Property(e => e.SpotAssistedKills).HasDefaultValue(0);
            builder.Property(e => e.SpotAssistedOnlyKills).HasDefaultValue(0);
            builder.Property(e => e.AssistedKills).HasDefaultValue(0);
            builder.Property(e => e.UnassistedKills).HasDefaultValue(0);
            builder.Property(e => e.UnassistedHeadshotKills).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedOnlyDeaths).HasDefaultValue(0);
            builder.Property(e => e.GrenadeAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.GrenadeAssistedOnlyDeaths).HasDefaultValue(0);
            builder.Property(e => e.SpotAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.SpotAssistedOnlyDeaths).HasDefaultValue(0);
            builder.Property(e => e.AssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.UnassistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.UnassistedHeadshotDeaths).HasDefaultValue(0);
        }
    }
}
