using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchTeamResultConfiguration : IEntityTypeConfiguration<ScrimMatchTeamResult>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchTeamResult> builder)
        {
            builder.ToTable("ScrimMatchTeamResult");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.TeamOrdinal
            });

            builder.Property(e => e.Points).HasDefaultValue(0);
            builder.Property(e => e.NetScore).HasDefaultValue(0);
            builder.Property(e => e.Kills).HasDefaultValue(0);
            builder.Property(e => e.Deaths).HasDefaultValue(0);
            builder.Property(e => e.Headshots).HasDefaultValue(0);
            builder.Property(e => e.HeadshotDeaths).HasDefaultValue(0);
            builder.Property(e => e.Suicides).HasDefaultValue(0);
            builder.Property(e => e.Teamkills).HasDefaultValue(0);
            builder.Property(e => e.TeamkillDeaths).HasDefaultValue(0);
            builder.Property(e => e.RevivesGiven).HasDefaultValue(0);
            builder.Property(e => e.RevivesTaken).HasDefaultValue(0);
            builder.Property(e => e.DamageAssists).HasDefaultValue(0);
            builder.Property(e => e.UtilityAssists).HasDefaultValue(0);
            builder.Property(e => e.DamageAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.UtilityAssistedDeaths).HasDefaultValue(0);
            builder.Property(e => e.ObjectiveCaptureTicks).HasDefaultValue(0);
            builder.Property(e => e.ObjectiveDefenseTicks).HasDefaultValue(0);
            builder.Property(e => e.BaseDefenses).HasDefaultValue(0);
            builder.Property(e => e.BaseCaptures).HasDefaultValue(0);


        }
    }
}


