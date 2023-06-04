using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Models.ScrimMatchReports;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchReportInfantryTeamRoundStatsConfiguration : IEntityTypeConfiguration<ScrimMatchReportInfantryTeamRoundStats>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchReportInfantryTeamRoundStats> builder)
        {
            builder.ToView("View_ScrimMatchReportInfantryTeamRoundStats");

            builder.HasNoKey();

            builder.Ignore(p => p.OneVsOneCount);
            builder.Ignore(p => p.OneVsOneKillDeathRatio);
            builder.Ignore(p => p.UnassistedKills);
            builder.Ignore(p => p.EventsAsHeavyAssault);
            builder.Ignore(p => p.EventsAsInfiltrator);
            builder.Ignore(p => p.EventsAsLightAssault);
            builder.Ignore(p => p.EventsAsMedic);
            builder.Ignore(p => p.EventsAsEngineer);
            builder.Ignore(p => p.EventsAsMax);
            builder.Ignore(p => p.EnemyDeaths);
            builder.Ignore(p => p.HeadshotPercent);
            builder.Ignore(p => p.HeadshotEnemyDeathPercent);
            builder.Ignore(p => p.SecuredKills);
            builder.Ignore(p => p.ConfirmedDeaths);

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.NetScore).HasDefaultValue(0);
            builder.Property(e => e.FacilityCapturePoints).HasDefaultValue(0);
            builder.Property(e => e.Kills).HasDefaultValue(0);
            builder.Property(e => e.HeadshotKills).HasDefaultValue(0);
            builder.Property(e => e.Deaths).HasDefaultValue(0);
            builder.Property(e => e.HeadshotEnemyDeaths).HasDefaultValue(0);
            builder.Property(e => e.TeamKills).HasDefaultValue(0);
            builder.Property(e => e.Suicides).HasDefaultValue(0);
            builder.Property(e => e.ScoredDeaths).HasDefaultValue(0);
            builder.Property(e => e.ZeroPointDeaths).HasDefaultValue(0);
            builder.Property(e => e.TeamKillDeaths).HasDefaultValue(0);
            builder.Property(e => e.TrickleDeaths).HasDefaultValue(0);
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
            builder.Property(e => e.DamageAssistsAsHeavyAssault).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsInfiltrator).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsLightAssault).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsMedic).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsEngineer).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistsAsMax).HasDefaultValue(0);
            builder.Property(e => e.KillDamageDealt).HasDefaultValue(0);
            builder.Property(e => e.AssistDamageDealt).HasDefaultValue(0);
            builder.Property(e => e.TotalDamageDealt).HasDefaultValue(0);
            builder.Property(e => e.Revives).HasDefaultValue(0);
            builder.Property(e => e.EnemyRevivesAllowed).HasDefaultValue(0);
            builder.Property(e => e.PeriodicControlTicks).HasDefaultValue(0);
            builder.Property(e => e.PeriodicControlTickPoints).HasDefaultValue(0);
            builder.Property(e => e.PostReviveKills).HasDefaultValue(0);
            builder.Property(e => e.ReviveInstantDeaths).HasDefaultValue(0);
            builder.Property(e => e.ReviveLivesMoreThan15s).HasDefaultValue(0);
            builder.Property(e => e.ShortestRevivedLifeSeconds).HasDefaultValue(0);
            builder.Property(e => e.LongestRevivedLifeSeconds).HasDefaultValue(0);
            builder.Property(e => e.AvgRevivedLifeSeconds).HasDefaultValue(0);

        }
    }
}
