using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using squittal.ScrimPlanetmans.Data.Models;

namespace squittal.ScrimPlanetmans.Data.DataConfigurations
{
    public class ScrimMatchTeamPointAdjustmentConfiguration : IEntityTypeConfiguration<ScrimMatchTeamPointAdjustment>
    {
        public void Configure(EntityTypeBuilder<ScrimMatchTeamPointAdjustment> builder)
        {
            builder.ToTable("ScrimMatchTeamPointAdjustment");

            builder.HasKey(e => new
            {
                e.ScrimMatchId,
                e.TeamOrdinal,
                e.Timestamp
            });

            builder.HasOne(teamAdjustment => teamAdjustment.ScrimMatchTeamResult)
                .WithMany(teamResult => teamResult.PointAdjustments)
                .HasForeignKey(teamAdjustment => new { teamAdjustment.ScrimMatchId, teamAdjustment.TeamOrdinal });

            builder.Property(e => e.Points).HasDefaultValue(0);
        }
    }
}

